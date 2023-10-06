namespace UsersApi.Exeptions
{


    /// <summary>
    /// исключение выплывающее при отсутствии пользователя по заданым критериям
    /// </summary>
    public class NoUserExeption : Exception
    {

        /// <summary>
        /// сообщение об ошибке
        /// </summary>
        public override string Message { get;}

        /// <summary>
        /// конструктор исключения
        /// </summary>
        public NoUserExeption(string message) {
            this.Message = message;
        }
    }
}
