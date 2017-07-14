using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using api.Controllers;
using System.Resources;
using api.Resources;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Net.Mime;

namespace api.Resources
{
    public class Streaming
    {
        /// <summary>
        /// Obsolete method
        /// </summary>
        static Streaming()
        {
            
            var mimeNames = new Dictionary<string, string>
            {
                { ".mp3", "audio/mpeg" },    // List all supported media types; 
                { ".mp4", "video/mp4" },
                { ".ogg", "application/ogg" },
                { ".ogv", "video/ogg" },
                { ".oga", "audio/ogg" },
                { ".wav", "audio/x-wav" },
                { ".webm", "video/webm" }
            };
            MimeNames = new ReadOnlyDictionary<string, string>(mimeNames);

            InvalidFileNameChars = Array.AsReadOnly(Path.GetInvalidFileNameChars());
            //InitialDirectory = WebConfigurationManager.AppSettings["InitialDirectory"];
        }

        private const int ReadStreamBufferSize = 1024 * 1024;
        private static readonly IReadOnlyDictionary<string, string> MimeNames;
        private static readonly IReadOnlyCollection<char> InvalidFileNameChars;
        // Where are videos are located 
        public static readonly string InitialDirectory;
        // Response to end user
        private static HttpResponseMessage response;
        //File information
        private static FileInfo MovieFileInfo;

        public static async Task<HttpResponseMessage> Content(Movie_Data movie, RangeHeaderValue header)
        {
            if (movie == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            string path = Get.MoviePath(movie);

            if (path == null)
                throw new HttpResponseException(HttpStatusCode.NoContent);

            MovieFileInfo = new FileInfo(Path.Combine(path, movie.name + "." + movie.ext));

            if (!MovieFileInfo.Exists)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            long totalLength = MovieFileInfo.Length;

            RangeHeaderValue rangeHeader = header;
            response = new HttpResponseMessage();

            response.Headers.AcceptRanges.Add("bytes");

            // The request will be treated as normal request if there is no Range header.
            if (rangeHeader == null || !rangeHeader.Ranges.Any())
            {

                response.StatusCode = HttpStatusCode.OK;
                response.Content = await Contains.RangeHeader();
                response.Content.Headers.ContentLength = totalLength;
                await History.Create(History.Type.API, new History_API()
                {
                    api_action = "Movie " + movie.Movie_Info.title + " served successfully.",
                    api_type = "Task -> Streaming movie ",
                    api_datetime = DateTime.Now
                });
                return response;
            }

            long start = 0, end = 0;

            // 1. If the unit is not 'bytes'.
            // 2. If there are multiple ranges in header value.
            // 3. If start or end position is greater than file length.
            if (rangeHeader.Unit != "bytes" || rangeHeader.Ranges.Count > 1 ||
                !Read.RangeItem(rangeHeader.Ranges.First(), totalLength, out start, out end))
            {

                response.StatusCode = HttpStatusCode.RequestedRangeNotSatisfiable;
                response.Content = new StreamContent(Stream.Null);  // No content for this status.
                response.Content.Headers.ContentRange = new ContentRangeHeaderValue(totalLength);
                response.Content.Headers.ContentType = Get.MimeNameFromExt(MovieFileInfo.Extension);
                await History.Create(History.Type.API, new History_API()
                {
                    api_action = "Movie " + movie.Movie_Info.title + " served successfully.",
                    api_type = "Task -> Streaming movie ",
                    api_datetime = DateTime.Now
                });
                return response;
            }

            var contentRange = new ContentRangeHeaderValue(start, end, totalLength);

            // PartialContent creator
            response.StatusCode = HttpStatusCode.PartialContent;

            response.Content = await Create.PartialContent(start, end);

            response.Content.Headers.ContentLength = end - start + 1;
            response.Content.Headers.ContentRange = contentRange;
            await History.Create(History.Type.API, new History_API()
            {
                api_action = "Movie " + movie.Movie_Info.title + " served successfully.",
                api_type = "Task -> Streaming movie ",
                api_datetime = DateTime.Now
            });
            return response;
        }


        public class Get
        {
            public static MediaTypeHeaderValue MimeNameFromExt(string ext)
            {
                string value = null;

                if(MimeNames.TryGetValue(ext.ToLowerInvariant(), out value)) 
                    return new MediaTypeHeaderValue(value);
                else
                    return new MediaTypeHeaderValue(MediaTypeNames.Application.Octet);
            }

            public static string MoviePath(Movie_Data movie)
            {
                foreach (var mDir in Global.Global.GlobalMovieDisksList)
                {
                    if (Directory.Exists(mDir.value + @"\" + movie.folder))
                    {
                        return mDir.value + @"\" + movie.folder;
                    }
                }
                return null;
            }
        }

        public class Read
        {
            public static bool RangeItem(
                RangeItemHeaderValue range, 
                long contentLength,
                out long start, 
                out long end)
            {
                if (range.From != null)
                {
                    start = range.From.Value;
                    if (range.To != null)
                        end = range.To.Value;
                    else
                        end = contentLength - 1;
                }
                else
                {
                    end = contentLength - 1;
                    if (range.To != null)
                        start = contentLength - range.To.Value;
                    else
                        start = 0;
                }
                return (start < contentLength && end < contentLength);
            }
        }

        public class Create
        {
            public static async Task WriteToOutputStream(Stream inputStream, Stream outputStream,
            long start, long end)
            {
                int count = 0;
                long remainingBytes = end - start + 1;
                long position = start;
                byte[] buffer = new byte[ReadStreamBufferSize];

                inputStream.Position = start;
                do
                {
                    try
                    {
                        if (remainingBytes > ReadStreamBufferSize)
                            count = await inputStream.ReadAsync(buffer, 0, ReadStreamBufferSize);
                        else
                            count = await inputStream.ReadAsync(buffer, 0, (int)remainingBytes);
                        await outputStream.WriteAsync(buffer, 0, count);
                    }
                    catch (Exception ex)
                    {
                        await History.Create(History.Type.API, new History_API()
                        {
                            api_action = "Exception caught | Message " + ex.Message,
                            api_type = "Exception -> MediaLibrary.CreatePartialContent()",
                            api_datetime = DateTime.Now
                        });
                        break;
                    }
                    position = inputStream.Position;
                    remainingBytes = end - position + 1;
                } while (position <= end);
            }

            public static async Task<PushStreamContent> PartialContent(long start,long end)
            {
                await Task.Delay(0);
                return new PushStreamContent(async (outputStream, httpContent, transpContext)
                =>
                {
                    try
                    {
                        using (outputStream) 
                        using (Stream inputStream = MovieFileInfo.OpenRead())
                        {
                            await WriteToOutputStream(inputStream, outputStream, start, end);
                        }
                    }
                    catch(Exception ex)
                    {
                        await History.Create(History.Type.API, new History_API() {
                            api_action = "Exception error -> " +ex.Message,
                            api_type = "Exception on Streaming.Create.PartialContent"
                        });
                    }
                    
                }, Get.MimeNameFromExt(MovieFileInfo.Extension));
            }
        }

        public class Contains
        {
            public static bool AnyInvalidFileNameChars(string fileName)
            {
                return InvalidFileNameChars.Intersect(fileName).Any();
            }

            public static async Task<PushStreamContent> RangeHeader()
            {
                await Task.Delay(0);
                return new PushStreamContent(async (outputStream, httpContent, transpContext)
                =>
                {
                    using (outputStream) // Copy the file to output stream straightforward. 
                    using (Stream inputStream = MovieFileInfo.OpenRead())
                    {
                        try
                        {
                            await inputStream.CopyToAsync(outputStream, ReadStreamBufferSize);
                        }
                        catch (Exception e)
                        {
                            await History.Create(History.Type.API, new History_API()
                            {
                                api_action = "Exception caught | Message " + e.Message,
                                api_type = "Exception -> MoviesAPI.Get.MovieInfo()",
                                api_datetime = DateTime.Now
                            });
                        }
                    }
                }, Get.MimeNameFromExt(MovieFileInfo.Extension));
            }
        }
    }
}