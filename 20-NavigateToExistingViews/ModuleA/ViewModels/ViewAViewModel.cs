using Prism.Mvvm;
using Prism.Navigation.Regions;

namespace ModuleA.ViewModels
{
    public class ViewAViewModel : BindableBase, INavigationAware
    {
        private string _title = "ViewA";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private int _pageViews;
        public int PageViews
        {
            get { return _pageViews; }
            set { SetProperty(ref _pageViews, value); }
        }

        public ViewAViewModel()
        {

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            PageViews++;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            // 不等于1时，可以重用当前试图；
            // 等于1时，创建新的试图实例
            return PageViews / 3 != 1;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
    }
}
