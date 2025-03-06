
## 集中问题



1. 接口 和 abstract
2. Ribbon，DropdownList
3. 案例
4. [Dependency Injection with Prism](https://docs.prismlibrary.com/docs/dependency-injection)
5. DevExpress


### Mvvm

MVVM 的核心原则：

- View 只负责显示
- ViewModel 包含业务逻辑
- Model 包含数据
- 各层之间应该松耦合

最佳实践：

- 优先使用 Commands 和 Triggers
- 将业务逻辑放在 ViewModel 中
- 使用 RelayCommand 或 DelegateCommand 实现命令
- 保持 View 的简单性
- 确保 ViewModel 的可测试性

### [TODO] 接口 和 abstract class

案例： `Person : INotifyPropertyChanged`
```c#
namespace System.ComponentModel
{
    //
    // 摘要:
    //     Notifies clients that a property value has changed.
    public interface INotifyPropertyChanged
    {
        //
        // 摘要:
        //     Occurs when a property value changes.
        event PropertyChangedEventHandler? PropertyChanged;
    }
}

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ModuleA.Business
{
    public class Person : INotifyPropertyChanged
    {
        #region Properties

        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                OnPropertyChanged();
            }
        }

        private string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                OnPropertyChanged();
            }
        }

        private int _age;
        public int Age
        {
            get { return _age; }
            set
            {
                _age = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _lastUpdated;
        public DateTime? LastUpdated
        {
            get { return _lastUpdated; }
            set
            {
                _lastUpdated = value;
                OnPropertyChanged();
            }
        }

        #endregion //Properties

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName]string propertyname = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        #endregion //INotifyPropertyChanged

        public override string ToString()
        {
            return String.Format("{0}, {1}", LastName, FirstName);
        }
    }
}

```


Prism中  `BindableBase`
```C#
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Prism.Mvvm
{
    /// <summary>
    /// Implementation of <see cref="INotifyPropertyChanged"/> to simplify models.
    /// </summary>
    public abstract class BindableBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            RaisePropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <param name="onChanged">Action that is called after the property value has been changed.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, Action? onChanged,
            [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            onChanged?.Invoke();
            RaisePropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="args">The PropertyChangedEventArgs</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }
    }
}

```

使用案例：`TabViewModel : BindableBase`

```c#
using Prism.Commands;
using Prism.Mvvm;
using System;
using UsingCompositeCommands.Core;

namespace ModuleA.ViewModels
{
    public class TabViewModel : BindableBase
    {
        IApplicationCommands _applicationCommands;

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private bool _canUpdate = true;
        public bool CanUpdate
        {
            get { return _canUpdate; }
            set { SetProperty(ref _canUpdate, value); }
        }

        private string _updatedText;
        public string UpdateText
        {
            get { return _updatedText; }
            set { SetProperty(ref _updatedText, value); }
        }

        public DelegateCommand UpdateCommand { get; private set; }

        public TabViewModel(IApplicationCommands applicationCommands)
        {
            _applicationCommands = applicationCommands;

            UpdateCommand = new DelegateCommand(Update).ObservesCanExecute(() => CanUpdate);

            _applicationCommands.SaveCommand.RegisterCommand(UpdateCommand);
        }

        private void Update()
        {
            UpdateText = $"Updated: {DateTime.Now}";
        }       
    }
}

```


`abstract class`, `interface`, `virtual`


### Trigger/Command和事件 和 数据绑定

是的，您理解得很正确。在 MVVM 模式中，确实更推荐使用 Triggers 和 Commands 而不是传统的事件处理。让我详细解释原因：

1. **MVVM 的核心原则**：
   - View 只负责显示
   - ViewModel 包含业务逻辑
   - Model 包含数据
   - 各层之间应该松耦合

2. **传统事件处理的问题**：
```csharp
// 不好的做法
public partial class MainWindow : Window
{
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        // 直接处理UI事件
        // 混合了UI逻辑和业务逻辑
        // 难以测试
        // 难以复用
    }
}
```

3. **MVVM 推荐的做法**：
```xml
<!-- XAML -->
<Button Command="{Binding SaveCommand}" />
```

```csharp
// ViewModel
public class MainViewModel
{
    public ICommand SaveCommand { get; }
    
    public MainViewModel()
    {
        SaveCommand = new RelayCommand(ExecuteSave);
    }
    
    private void ExecuteSave()
    {
        // 纯业务逻辑
        // 易于测试
        // 易于复用
    }
}
```

4. **使用 Triggers 的优势**：
   - **解耦**：View 和 ViewModel 完全分离
   - **可测试性**：命令可以直接在单元测试中测试
   - **可复用性**：命令可以在不同视图中重用
   - **可维护性**：逻辑集中在 ViewModel 中
   - **灵活性**：可以轻松添加条件判断和参数传递

5. **常见的使用场景**：
```xml

<!-- 
Interaction.Triggers：交互触发器的容器
EventTrigger：定义要监听的事件
InvokeCommandAction：定义事件触发时要执行的动作 
-->

<!-- 按钮点击 -->
<Button>
    <i:Interaction.Triggers>
        <i:EventTrigger EventName="Click">
            <i:InvokeCommandAction Command="{Binding SubmitCommand}" />
        </i:EventTrigger>
    </i:Interaction.Triggers>
</Button>

<!-- 文本改变 -->
<TextBox>
    <i:Interaction.Triggers>
        <i:EventTrigger EventName="TextChanged">
            <i:InvokeCommandAction Command="{Binding TextChangedCommand}" />
        </i:EventTrigger>
    </i:Interaction.Triggers>
</TextBox>

<!-- 选择改变 -->
<ListBox>
    <i:Interaction.Triggers>
        <i:EventTrigger EventName="SelectionChanged">
            <i:InvokeCommandAction Command="{Binding SelectionChangedCommand}" />
        </i:EventTrigger>
    </i:Interaction.Triggers>
</ListBox>
```

6. **特殊情况**：
虽然 MVVM 推荐使用 Commands 和 Triggers，但也有一些特殊情况可能需要使用事件：
   - 处理复杂的 UI 交互
   - 处理第三方控件的特殊事件
   - 处理一些底层的 UI 事件

7. **最佳实践**：
   - 优先使用 Commands 和 Triggers
   - 将业务逻辑放在 ViewModel 中
   - 使用 RelayCommand 或 DelegateCommand 实现命令
   - 保持 View 的简单性
   - 确保 ViewModel 的可测试性

总的来说，在 MVVM 模式中，使用 Triggers 和 Commands 是更好的选择，因为它们：
- 更好地支持 MVVM 的设计理念
- 提供更好的可测试性
- 提供更好的可维护性
- 提供更好的可复用性
- 使代码更加清晰和模块化

这就是为什么在现代 WPF 应用程序中，我们更倾向于使用 Triggers 和 Commands 而不是传统的事件处理方式。


```xml
<!-- 直接绑定到 View对应的ViewModel 的属性 -->
CommandParameter="{Binding CurrentPerson}"

<!-- 绑定到当前数据上下文 -->
 <!-- 它会传递整个 ViewModel 作为命令参数 -->
CommandParameter="{Binding}"

<!-- 使用多重绑定 -->
CommandParameter="{Binding SelectedItem, ElementName=_listOfPeople, Converter={StaticResource MyConverter}}"
```

```xml
<!-- ViewA.xaml -->


<!-- SelectedItem 是要绑定的属性名
ElementName=_listOfPeople 指定了绑定的源元素 -->

<!-- {Binding People} 表示绑定到ViewA.xaml对应的ViewAViewModel.cs中的属性People -->
 <!-- SelectionChanged 表示 要监听的事件 -->
 <!-- PersonSelectedCommand表示执行的动作，绑定到ViewAViewModel.cs中的PersonSelectedCommand -->
 <!-- {Binding SelectedItem, ElementName=_listOfPeople} 表示传递进去的参数。绑定到_listOfPeople的SelectedItem属性 -->

 <!-- EventTrigger：定义要监听的事件
InvokeCommandAction：定义事件触发时要执行的动作 -->


        <ListBox x:Name="_listOfPeople" ItemsSource="{Binding People}">            
            <i:Interaction.Triggers>
                <i:EventTrigger EventName="SelectionChanged">

                    <prism:InvokeCommandAction Command="{Binding PersonSelectedCommand}" CommandParameter="{Binding SelectedItem, ElementName=_listOfPeople}" />
                </i:EventTrigger>
            </i:Interaction.Triggers>
        </ListBox>
```


**CommandParameter="{Binding}**
```xml
<!-- XAML -->
<Button Command="{Binding SaveCommand}" 
        CommandParameter="{Binding}">
    <i:Interaction.Triggers>
        <i:EventTrigger EventName="Click">
            <i:InvokeCommandAction Command="{Binding SaveCommand}" 
                                 CommandParameter="{Binding}" />
        </i:EventTrigger>
    </i:Interaction.Triggers>
</Button>
```

```cs
// ViewModel
public class MainViewModel
{
    public ICommand SaveCommand { get; }

    public MainViewModel()
    {
        // 命令参数类型是 MainViewModel
        SaveCommand = new DelegateCommand<MainViewModel>(ExecuteSave);
    }

    private void ExecuteSave(MainViewModel viewModel)
    {
        // 这里可以直接访问整个 ViewModel
        // 可以访问所有属性和方法
        var name = viewModel.Name;
        var age = viewModel.Age;
        // ... 其他操作
    }
}
```


**{Binding SelectedItem, ElementName=_listOfPeople}**

```xml
<!-- ListBox 定义 -->
<ListBox x:Name="_listOfPeople" ItemsSource="{Binding People}">
    <i:Interaction.Triggers>
        <i:EventTrigger EventName="SelectionChanged">
            <!-- 当选择改变时，将选中的项目传递给命令 -->
            <prism:InvokeCommandAction 
                Command="{Binding PersonSelectedCommand}" 
                CommandParameter="{Binding SelectedItem, ElementName=_listOfPeople}" />
        </i:EventTrigger>
    </i:Interaction.Triggers>
</ListBox>
```

```cs
public class PersonListViewModel
{
    public ICommand PersonSelectedCommand { get; }

    public PersonListViewModel()
    {
        PersonSelectedCommand = new DelegateCommand<Person>(ExecutePersonSelected);
    }

    private void ExecutePersonSelected(Person selectedPerson)
    {
        // selectedPerson 就是从 ListBox 传递过来的选中项
        // 可以在这里处理选中的 Person 对象
    }
}
```

**Convertor**

```cs
// 布尔值转换器
public class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? Visibility.Visible : Visibility.Collapsed;
    }
}

// 日期转换器
public class DateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }
        return string.Empty;
    }
}

// 数值范围转换器
public class AgeRangeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int age)
        {
            if (age < 18) return "未成年";
            if (age < 30) return "青年";
            if (age < 50) return "中年";
            return "老年";
        }
        return "未知";
    }
}
```

```xml
<UserControl.Resources>
    <!-- 注册多个转换器 -->
    <local:BooleanToVisibilityConverter x:Key="BoolToVisibilityConverter"/>
    <local:DateConverter x:Key="DateConverter"/>
    <local:AgeRangeConverter x:Key="AgeConverter"/>
</UserControl.Resources>

<!-- 使用转换器 -->
<TextBlock Visibility="{Binding IsVisible, Converter={StaticResource BoolToVisibilityConverter}}"/>
<TextBlock Text="{Binding BirthDate, Converter={StaticResource DateConverter}}"/>
<TextBlock Text="{Binding Age, Converter={StaticResource AgeConverter}}"/>
```



## 20250305-IEventAggregator

主题：IEventAggregator

视频：Lesson9

作用 ：

- 可以在2个 ViewModel/View/Object 中间通讯。
- 这是Event-Base Communication System.。 It Works by having publishers(publish event) and subscribers.
- 支持多publisher和多subscribers
- parameters，也叫payloads


## 20250305-region navigation

视频：Lesson10

同一个区域，按不同按钮显示不同内容。这个功能就是导航。prism会自动处理Move过程(activate/deactivate)过程.

navigation的作用：
1. add view to region（这一点和普通的region一样）
2. 自动处理move from a view to another，包括自动activate/deactivate等
3. 基于URI方式


IsNavigationTarget：

- false:重新实例化view/viewModel
- true:重用已有的view/viewModel

View(.xaml) + ViewModel(.cs) 构成 一个 "view"
这个view可以被放入到region中。

默认情况下，RequestNavigate 会创建一个新的 View 放到 Region 中。

调用时机：
1.	导航到视图：
    • IsNavigationTarget：首先调用，决定是否重用现有视图实例。
    • OnNavigatedTo：如果导航到当前视图，则调用此方法进行初始化或更新操作。
2.	导航离开视图：
    • OnNavigatedFrom：当导航离开当前视图时调用，用于执行清理或保存操作。

第一次切导航的时候：`_regionManager.RequestNavigate("PersonDetailsRegion", "PersonDetail", parameters); `的时候，测试发现：

- 会直接调用OnNavigatedTo，而不会调用IsNavigationTarget。

第2切导航的时候，发现会先调用IsNavigationTarget，然后再调用OnNavigatedTo

RequestNavigate 会尝试在区域中创建新的视图
- 如果区域是 TabControl，会自动创建新的 TabItem
- 是否创建新的 TabItem 取决于：
- IsNavigationTarget 的返回值
    - 当前视图的状态
    - 导航参数
    - 可以通过 IsNavigationTarget 控制视图重用
关键点：
- 导航会创建新的视图
- 区域类型决定视图的容器（这里是 TabItem）
- IsNavigationTarget 控制是否重用视图
- 每个导航请求都可能创建新的 TabItem
- 视图重用可以避免创建不必要的 TabItem


总结，有同一个区域切换view的情况可以使用，更方便一点。比如：
1. 常规的换画面。 <ContentControl Margin="5" prism:RegionManager.RegionName="ContentRegion" />
2. TabList中的TabItem处理。 <TabControl Margin="5" prism:RegionManager.RegionName="ContentRegion" />

DataContext，就是这个类的依赖属性，可以和前端进行数据绑定。

```xml
    <Window.Resources>
        <Style TargetType="TabItem">
            <Setter Property="Header" Value="{Binding DataContext.Title}" />
        </Style>
    </Window.Resources>
```

一般的类，可以继承自 INotifyPropertyChanged。
view中的类，在Prism框架下，可以继承自： `public class PersonDetailViewModel : BindableBase`


## 20250306 dialog


model类型(ShowDialog)：必须关闭才能返回主界面
non-model类型(show)：可以和主界面自由点击
prism中，dialog是个user control


## 其他

**TriggerParameterPath="AddedItems" 到底是啥，就是指定输入参数的意思么。如果是的花，为啥不用：CommandParameter**

TriggerParameterPath:
- 用于从事件参数中提取特定属性值
- 在事件触发时动态获取值
- 指向事件参数对象中的属性路径

CommandParameter:
- 用于传递静态值或绑定值
- 值是预先确定的，不依赖于事件参数
- 通常用于传递固定值或视图模型中的属性

这里的 AddedItems 是 SelectionChangedEventArgs 类中的一个属性：

```xml
        <ListBox Grid.Row="1" Margin="5" ItemsSource="{Binding Items}" SelectionMode="Single">
            <i:Interaction.Triggers>
                <!-- This event trigger will execute the action when the corresponding event is raised by the ListBox. -->
                <i:EventTrigger EventName="SelectionChanged">
                    <!-- This action will invoke the selected command in the view model and pass the parameters of the event to it. -->
                    <prism:InvokeCommandAction Command="{Binding SelectedCommand}" TriggerParameterPath="AddedItems" />
                </i:EventTrigger>
            </i:Interaction.Triggers>
        </ListBox>
```


## 参考资料

Prism提供了包括MVVM、依赖注入、命令和事件聚合器等设计模式，这些设计模式有助于编写效果良好且可维护的XAML应用程序

- [Empower Your XAML Applications](https://prismlibrary.com/)
  - [Introduction to Prism 7 for WPF](https://www.pluralsight.com/courses/prism-wpf-introduction)(培训课程)
- [Introduction to Prism](https://prismlibrary.github.io/docs/)
  - [Introduction to Prism](https://docs.prismlibrary.com/docs/index.xml)
  - [Prism简介](https://csharpshare.com/articles/framework/prism-doc/index.xml)
  - [NuGet包中库的介绍](https://csharpshare.com/articles/framework/prism-doc/getting-started/NuGet-Packages.xml)
- [Github-Prism](https://github.com/PrismLibrary/Prism)
  - [Prism-Samples-Wpf](https://github.com/PrismLibrary/Prism-Samples-Wpf)

