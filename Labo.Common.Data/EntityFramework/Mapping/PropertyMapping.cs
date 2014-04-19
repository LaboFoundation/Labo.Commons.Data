namespace Labo.Common.Data.EntityFramework.Mapping
{
    /// <summary>
    /// Property mapping class.
    /// </summary>
    public sealed class PropertyMapping
    {
        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        public string ColumnName { get; set; }
    }
}