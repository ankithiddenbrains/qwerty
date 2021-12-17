namespace Tracer.API.Helper
{
    public static class ResponseMessage
    {
        /// <summary>
        /// The message success
        /// </summary>
        public static string Success = "Success";

        /// <summary>
        /// The message success
        /// </summary>
        public static string Failure = "Failure";


        /// <summary>
        /// The message success
        /// </summary>
        public static string InvalidParameters = "Invalid request parametrs";

        /// <summary>
        /// The email not exists
        /// </summary>
        public static string EmailNotExists = "Email does not exists";


        /// <summary>
        /// The incorrect password
        /// </summary>
        public static string IncorrectPassword = "Incorrect Password";

        /// <summary>
        /// Gets or sets the invalid requests model.
        /// </summary>
        /// <value>
        /// The invalid requests model.
        /// </value>
        public static string Invalid_Requests_Model { get; set; } = "Request model is not valid";

        /// <summary>
        /// The email not exists
        /// </summary>
        public static string EmailSuccess= "Email send successfully";

        /// <summary>
        /// The email not exists
        /// </summary>
        public static string EmailFailure = "Problem in sending email";

        /// <summary>
        /// The email not exists
        /// </summary>
        public static string NoRecordFound = "No Record Found";

        /// <summary>
        /// The incorrect credentails
        /// </summary>
        public static string IncorrectCredentails = "Incorrect Credentials";

    }
}
