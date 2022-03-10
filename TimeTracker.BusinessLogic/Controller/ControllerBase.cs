using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TimeTracker.BusinessLogic.Controller
{
    public abstract class ControllerBase
    {
        /// <summary>
        /// Save data to file.
        /// </summary>
        /// <param name="fileName"> The name of the file where the data will be saved. </param>
        /// <param name="item"> Object to save. </param>
        public void Save(string fileName, object item)
        {
            var formatter = new BinaryFormatter();

            using (var fileStream = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fileStream, item);
            }
        }

        /// <summary>
        /// Returns loaded data or default value for T.
        /// </summary>
        /// <typeparam name="T"> Type of loading data. </typeparam>
        /// <param name="fileName"> The name of the file from which the data will be loaded. </param>
        protected T Load<T>(string fileName)
        {
            var formatter = new BinaryFormatter();

            using (var fileStream = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                if (fileStream.Length > 0 && formatter.Deserialize(fileStream) is T items)
                    return items;
                return default;
            }
        }
    }
}
