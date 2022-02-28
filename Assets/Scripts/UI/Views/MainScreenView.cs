using System.Collections.Generic;

namespace UI.Views
{
    public class MainScreenView : UIBase<MainScreenView.MainScreenData>
    {
        public struct MainScreenData : IUIData
        {
            public List<string> testData;
        }

        public override void SetData(MainScreenData data)
        {
            throw new System.NotImplementedException();
        }
    }
}