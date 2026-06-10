namespace System
{
    public static class GameMessages
    {
        public static string WELCOME(string name) => $"WElCOME player {name}";
        public static string Alert(string alertMessage) => alertMessage;
    }
}
