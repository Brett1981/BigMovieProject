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

namespace api.Resources
{
    public class Streaming
    {
        public static HttpResponseMessage streamingContent(MovieData movie, RangeHeaderValue header)
        {
            // This can prevent some unnecessary accesses. 
            // These kind of file names won't be existing at all. 
            if (movie == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            string path = "";
            /*if (VideoController.streamDir.EndsWith(@"\")) {path = VideoController.streamDir + movie.movie_ext + @"\"; }
            else { path = VideoController.streamDir + @"\" + movie.movie_ext + @"\"; }*/

            foreach(var mDir in VideoController.movieDir)
            {
                if(Directory.Exists(mDir + @"\"+ movie.movie_folder)) { path = mDir + @"\" + movie.movie_folder; break; }
                if(path != "") { break;}
            }
            /*if (VideoController.movieDir.EndsWith(@"\")) { path = VideoController.streamDir + movie.movie_name + @"\"; }
            else { path = VideoController.streamDir + @"\" + movie.movie_folder + @"\"; }*/

            FileInfo fileInfo = new FileInfo(Path.Combine(path, movie.movie_name + "." + movie.movie_ext));
            //FileInfo fileInfo = new FileInfo(file);

            if (!fileInfo.Exists)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            long totalLength = fileInfo.Length;

            RangeHeaderValue rangeHeader = header;
            HttpResponseMessage response = new HttpResponseMessage();

            response.Headers.AcceptRanges.Add("bytes");

            // The request will be treated as normal request if there is no Range header.
            if (rangeHeader == null || !rangeHeader.Ranges.Any())
            {
                response.StatusCode = HttpStatusCode.OK;
                response.Content = new PushStreamContent((outputStream, httpContent, transpContext)
                =>
                {
                    using (outputStream) // Copy the file to output stream straightforward. 
                    using (Stream inputStream = fileInfo.OpenRead())
                    {
                        try
                        {
                            inputStream.CopyTo(outputStream, MediaLibrary.ReadStreamBufferSize);
                        }
                        catch (Exception error)
                        {
                            Debug.WriteLine(error);
                        }
                    }
                }, MediaLibrary.GetMimeNameFromExt(fileInfo.Extension));

                response.Content.Headers.ContentLength = totalLength;
                return response;
            }

            long start = 0, end = 0;

            // 1. If the unit is not 'bytes'.
            // 2. If there are multiple ranges in header value.
            // 3. If start or end position is greater than file length.
            if (rangeHeader.Unit != "bytes" || rangeHeader.Ranges.Count > 1 ||
                !MediaLibrary.TryReadRangeItem(rangeHeader.Ranges.First(), totalLength, out start, out end))
            {
                response.StatusCode = HttpStatusCode.RequestedRangeNotSatisfiable;
                response.Content = new StreamContent(Stream.Null);  // No content for this status.
                response.Content.Headers.ContentRange = new ContentRangeHeaderValue(totalLength);
                response.Content.Headers.ContentType = MediaLibrary.GetMimeNameFromExt(fileInfo.Extension);

                return response;
            }

            var contentRange = new ContentRangeHeaderValue(start, end, totalLength);

            // We are now ready to produce partial content.
            response.StatusCode = HttpStatusCode.PartialContent;
            response.Content = new PushStreamContent((outputStream, httpContent, transpContext)
            =>
            {
                using (outputStream) // Copy the file to output stream in indicated range.
                using (Stream inputStream = fileInfo.OpenRead())
                    MediaLibrary.CreatePartialContent(inputStream, outputStream, start, end);

            }, MediaLibrary.GetMimeNameFromExt(fileInfo.Extension));

            response.Content.Headers.ContentLength = end - start + 1;
            response.Content.Headers.ContentRange = contentRange;

            return response;
        }
    
    }
}