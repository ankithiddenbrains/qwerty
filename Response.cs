using System;
using System.Collections.Generic;

namespace Tracer.API.Helper
{
    /// <summary>
    /// ResponseModel
    /// </summary>
    public class ResponseModel
    {
        #region Response
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Response"/> is status.
        /// </summary>
        /// <value>
        ///   <c>true</c> if status; otherwise, <c>false</c>.
        /// </value>
        public bool Status { get; set; } = false;

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public object Token { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public Object Data { get; set; }

        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        /// <value>
        /// The total count.
        /// </value>
        public int TotalCount { get; set; }
        #endregion
    }
    /// <summary>
    /// ResponseModel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseModel<T>
    {
        #region Response<T>
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Response{T}"/> is status.
        /// </summary>
        /// <value>
        ///   <c>true</c> if status; otherwise, <c>false</c>.
        /// </value>
        public bool Status { get; set; } = false;

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public ResultModel<T> List { get; set; }

        #endregion
    }

    public class ResultModel<T>
    {
        #region Result<T>
        /// <summary>
        /// Gets or sets the list items.
        /// </summary>
        /// <value>
        /// The list items.
        /// </value>
        public IEnumerable<T> List { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public Object Items { get; set; }

        public string Result { get; set; }

        #endregion
    }
    public class CommonResponse<T>
    {
        #region CommonResponse
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CommonResponse{T}"/> is status.
        /// </summary>
        /// <value>
        ///   <c>true</c> if status; otherwise, <c>false</c>.
        /// </value>
        public bool Status { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public T Data { get; set; }

        #endregion
    }

    public class ContainerList<T>
    {
        public ContainerList()
        {
            CntrList = new List<object>();
        }
        public List<object> CntrList { get; set; }
    }

}
