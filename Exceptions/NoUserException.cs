namespace UsersApi.Exeptions
{


    /// <summary>
    /// исключение выплывающее при отсутствии пользователя по заданым критериям
    /// </summary>
    public class NoUserException : Exception
    {

        /// <summary>
        /// сообщение об ошибке
        /// </summary>
        public override string Message { get;}

        /// <summary>
        /// конструктор исключения
        /// </summary>
        public NoUserException(string message) {
            this.Message = message;
        }
    }
}
