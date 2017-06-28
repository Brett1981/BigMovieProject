class Extensions{

    static JSONCheck(jsonData){
        try{
            return (JSON.parse(jsonData)) ? true: false;
        }
        catch(ex){
            return false;
        }
    }
}