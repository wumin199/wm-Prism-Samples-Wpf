using ModuleA.Business;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using System;
using System.Collections.ObjectModel;

namespace ModuleA.ViewModels
{
    public class PersonListViewModel : BindableBase
    {
        IRegionManager _regionManager;

        private ObservableCollection<Person> _people;
        public ObservableCollection<Person> People
        {
            get { return _people; }
            set { SetProperty(ref _people, value); }
        }

        public DelegateCommand<Person> PersonSelectedCommand { get; private set; }

        public PersonListViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            PersonSelectedCommand = new DelegateCommand<Person>(PersonSelected);
            CreatePeople();
        }

        // 和前端的： CommandParameter="{Binding SelectedItem, ElementName=_listOfPeople}" 对应
        private void PersonSelected(Person person)
        {
            //联系前端，是触发 SelectionChanged 才会调用PersonSelected
            var parameters = new NavigationParameters();

            // "person" 是ID， person 是值（object）
            parameters.Add("person", person);

            if (person != null)
                // regionName, viewName(Views/ViewA,ViewB, PersonDetail), parameters
                _regionManager.RequestNavigate("PersonDetailsRegion", "PersonDetail", parameters);
        }

        private void CreatePeople()
        {
            var people = new ObservableCollection<Person>();
            for (int i = 0; i < 10; i++)
            {
                people.Add(new Person()
                {
                    FirstName = String.Format("First {0}", i),
                    LastName = String.Format("Last {0}", i),
                    Age = i
                });
            }

            People = people;
        }
    }
}
