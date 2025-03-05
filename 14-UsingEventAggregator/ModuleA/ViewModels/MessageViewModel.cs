using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using UsingEventAggregator.Core;

namespace ModuleA.ViewModels
{
    public class MessageViewModel : BindableBase
    {
        IEventAggregator _ea;

        private string _message = "Message to Send";
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public DelegateCommand SendMessageCommand { get; private set; }

        public MessageViewModel(IEventAggregator ea)
        {
            _ea = ea;
            SendMessageCommand = new DelegateCommand(SendMessage);
        }

        private void SendMessage()
        {
            // 这个Message是来自于MessageViewModel的Message属性，也是来自于UI的输入
            _ea.GetEvent<MessageSentEvent>().Publish(Message);
        }
    }
}
