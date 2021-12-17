namespace Tracer.API.Helper.AppSetting
{
    #region ExternalUrl
    /// <summary>
    /// ExternalUrl
    /// </summary>
    public class ExternalUrl
    {
        /// <summary>
        /// Gets or sets the MVC project ip for erp.
        /// </summary>
        /// <value>
        /// The MVC project ip for erp.
        /// </value>
        public string MVCProjectIPForERP { get; set; }

        /// <summary>
        /// Gets or sets the tracer project ip.
        /// </summary>
        /// <value>
        /// The tracer project ip.
        /// </value>
        public string TracerProjectIP { get; set; }

        /// <summary>
        /// Gets or sets the angular path.
        /// </summary>
        /// <value>
        /// The angular path.
        /// </value>
        public string AngularPath { get; set; }
    }

    #endregion
}
