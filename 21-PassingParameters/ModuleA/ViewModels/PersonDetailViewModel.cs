using System;
using System.Xml;
using ModuleA.Business;
using Prism.Mvvm;
using Prism.Navigation.Regions;

namespace ModuleA.ViewModels
{
    public class PersonDetailViewModel : BindableBase, INavigationAware
    {
        private Person _selectedPerson;
        public Person SelectedPerson
        {
            get { return _selectedPerson; }
            set { SetProperty(ref _selectedPerson, value); }
        }

        public int RandNum { get; set; }

        public PersonDetailViewModel()
        {
            RandNum = new Random().Next(1, 100);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            // 这个方法在视图被导航到时被调用，你可以在这里执行任何需要在视图显示时进行的初始化或更新操作。
            var input_person = navigationContext.Parameters["person"] as Person;
            if (input_person != null)
                SelectedPerson = input_person;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            // true表示重用，不需要销毁和重新创建
            // false表示重新创建

            var check_person = navigationContext.Parameters["person"] as Person;
            if (check_person != null)
            {
                var rst = SelectedPerson != null && SelectedPerson.LastName == check_person.LastName;
                return rst;
            }
            else
                return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            // 这个方法在视图被导航离开时被调用，你可以在这里执行任何需要在视图隐藏时进行的清理或保存操作。

            // 在Prism框架中，导航离开视图时，视图的处理方式（隐藏或销毁）取决于具体的导航机制和区域管理器的配置。默认情况下，Prism会销毁视图，但你可以通过配置来改变这一行为。
        }
    }
}
