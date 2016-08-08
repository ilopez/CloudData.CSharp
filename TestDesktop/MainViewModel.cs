using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CloudDataClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestDesktop
{
    public class MainViewModel : BaseINPC
    {
        public String URL { get; set; }
        public String Key { get; set; }
        public Int32 Timeout { get; set; }

        public String Query { get; set; }

        public String Result { get; set; }

        public DataTable ResultDataTable { get; set; }

        public String ResultCode { get; set; }

        public ICommand Run => new RelayCommand(cmdRun);

        private void cmdRun()
        {
            CloudData cd = new CloudData(URL, Key);
            cd.SetTimeout(Timeout);

            var rs = cd.Query(Query);

            Result = rs.RawContent;
            try
            {
                //Object jObject = JsonConvert.DeserializeObject<JObject>(Result);
                ResultDataTable = JsonConvert.DeserializeObject<DataTable>(Result);
                // = data.Tables[0];

                StringBuilder sb = new StringBuilder();

                foreach (var i in ResultDataTable.Columns.Cast<DataColumn>())
                {
                    sb.AppendLine($"public {i.DataType.ToString()} {i.ColumnName} " + "{get ; set ; }");

                }

                ResultCode = sb.ToString();

            }
            catch (Exception ex)
            {            
                ResultDataTable = new DataTable();
            }

            OnPropertyChanged("Result");
            OnPropertyChanged("ResultCode");
            OnPropertyChanged("ResultDataTable");
        }
    }

    public abstract class BaseINPC : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }


    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        private Action methodToExecute;
        private Func<bool> canExecuteEvaluator;
        public RelayCommand(Action methodToExecute, Func<bool> canExecuteEvaluator)
        {
            this.methodToExecute = methodToExecute;
            this.canExecuteEvaluator = canExecuteEvaluator;
        }
        public RelayCommand(Action methodToExecute)
            : this(methodToExecute, null)
        {
        }
        public bool CanExecute(object parameter)
        {
            if (this.canExecuteEvaluator == null)
            {
                return true;
            }
            else
            {
                bool result = this.canExecuteEvaluator.Invoke();
                return result;
            }
        }
        public void Execute(object parameter)
        {
            this.methodToExecute.Invoke();
        }
    }
}
