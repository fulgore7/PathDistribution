using System;

namespace SPFramework.Data.Attributes
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Sets The name of the column in the table
        /// </summary>
        ///
        public string Name { get; protected set; }
    }
}