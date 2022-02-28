using Data.Net;
using VContainer;

namespace Data
{
    public class DataInitializer
    {
        [Inject] private IConfigLoader configLoader;

        public DataInitializer()
        {
            var json = configLoader.GetConfigJson();
            FillNodesFromJson(json);
        }

        private void FillNodesFromJson(string json)
        {

        }
    }
}