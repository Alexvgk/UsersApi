namespace UsersApi.Exeptions
{
    public class NoUserExeption : Exception
    {
        public override string Message { get;}
        public NoUserExeption(string message) {
            this.Message = message;
        }
    }
}
