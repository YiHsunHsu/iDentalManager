using iDentalManager.Class;
using iDentalManager.Commands;
using iDentalManager.DataTables;
using iDentalManager.Views.UserControls;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace iDentalManager.ViewModels
{
    public class MainWindowViewModel : ViewModelBase.PropertyChangedBase
    {
        #region FinctionTab
        /// <summary>
        /// binding Tab ItemSource來源
        /// </summary>
        private ObservableCollection<TabItem> functionsTabs = new ObservableCollection<TabItem>();
        public ObservableCollection<TabItem> FunctionsTabs
        {
            get { return functionsTabs; }
            set
            {
                functionsTabs = value;
                OnPropertyChanged("FunctionsTabs");
            }
        }

        /// <summary>
        /// Selected Tab頁面(載入圖片)
        /// </summary>
        private TabItem selectedTabItem;
        public TabItem SelectedTabItem
        {
            get { return selectedTabItem; }
            set
            {
                selectedTabItem = value;
                OnPropertyChanged("SelectedTabItem");
                switch (selectedTabItem.Uid)
                {
                    case "0":
                        selectedTabItem.Content = new FunctionServer();
                        break;
                    case "1":
                        selectedTabItem.Content = new FunctionClient();
                        break;
                }
            }
        }
        #endregion

        private string serverIP;
        public string ServerIP
        {
            get { return serverIP; }
            set
            {
                serverIP = value;
                OnPropertyChanged("ServerIP");
                if (ValidatorHelper.IsIP(serverIP))
                {
                    CheckIPConnection = DBAccess.CheckConnection(serverIP);
                }
                else
                {
                    CheckIPConnection = false;
                }
                OnPropertyChanged("ServerStatus");
            }
        }

        private BitmapImage serverStatus = new BitmapImage(new Uri(@"pack://application:,,,/iDentalManager;component/Resource/yes.png", UriKind.Absolute));
        public BitmapImage ServerStatus
        {
            get
            {
                if (!string.IsNullOrEmpty(ServerIP))
                {
                    if (CheckIPConnection)
                        return new BitmapImage(new Uri(@"pack://application:,,,/iDentalManager;component/Resource/yes.png", UriKind.Absolute));
                }
                return new BitmapImage(new Uri(@"pack://application:,,,/iDentalManager;component/Resource/no.png", UriKind.Absolute));
            }
        }

        private bool checkIPConnection = false;
        public bool CheckIPConnection
        {
            get { return checkIPConnection; }
            set
            {
                checkIPConnection = value;
                OnPropertyChanged("CheckIPConnection");
            }
        }

        private string agencyCode = string.Empty;
        public string AgencyCode
        {
            get { return agencyCode; }
            set
            {
                if (!agencyCode.Equals(value))
                {
                    agencyCode = value;
                    OnPropertyChanged("AgencyCode");
                    if (CheckIPConnection && agencyCode.Length == 10)
                    {
                        DatatableAgency = dTAgencys.QueryAgency(agencyCode);
                    }
                    else
                    {
                        IsAgencyExist = false;
                    }
                }
            }
        }

        private string verificationCode;
        public string VerificationCode
        {
            get { return verificationCode; }
            set
            {
                verificationCode = value;
                OnPropertyChanged("VerificationCode");
            }
        }

        private bool IsAgencyExist = false;
        private bool IsVerify = false;
        private bool IsTry = false;
        private string TrialPeriod = string.Empty;

        public string Tip
        {
            get
            {
                string TextTip = string.Empty;
                if (IsAgencyExist)
                {
                    TextTip = "機構代號:" + AgencyCode + "狀態:";
                    if (IsVerify)
                    {
                        if (IsTry)
                        {
                            TextTip += "為試用版，試用日期至" + TrialPeriod + "。";
                        }
                        else
                        {
                            TextTip += "啟用中。";
                        }
                    }
                    else
                    {
                        TextTip += "停用中。";
                    }
                }
                else
                {
                    TextTip = "機構代號:" + AgencyCode + "狀態:尚未註冊";
                }
                return TextTip;
            }
        }

        private DataTable datatableAgency = new DataTable();
        public DataTable DatatableAgency
        {
            get { return datatableAgency; }
            set
            {
                datatableAgency = value;
                if (datatableAgency != null)
                {
                    if (datatableAgency.Rows.Count > 0)
                    {
                        IsAgencyExist = true;
                        IsVerify = bool.Parse(datatableAgency.Rows[0]["Agency_IsVerify"].ToString());
                        TrialPeriod = string.IsNullOrEmpty(datatableAgency.Rows[0]["Agency_TrialPeriod"].ToString()) ? string.Empty : DateTime.Parse(datatableAgency.Rows[0]["Agency_TrialPeriod"].ToString()).ToShortDateString();
                        IsTry = bool.Parse(datatableAgency.Rows[0]["Agency_IsTry"].ToString());
                    }
                    else
                        IsAgencyExist = false;
                }
                else
                    IsAgencyExist = false;
                OnPropertyChanged("Tip");
            }
        }


        private DTAgencys dTAgencys;
        public MainWindowViewModel()
        {
            FunctionsSetting();
            dTAgencys = new DTAgencys();
        }

        private void FunctionsSetting()
        {
            for (int i = 0; i < 2; i++)
            {
                TabItem fTabItem = new TabItem();
                switch (i)
                {
                    case 0:
                        fTabItem.Header = "伺服器設定";
                        break;
                    case 1:
                        fTabItem.Header = "Client端設定";
                        break;
                }
                fTabItem.Uid = i.ToString();
                FunctionsTabs.Add(fTabItem);
            }

            SelectedTabItem = FunctionsTabs[0];
        }

        #region 註冊鈕
        private RelayCommand registerCommand;
        public ICommand RegisterCommand
        {
            get
            {
                if (registerCommand == null)
                {
                    registerCommand = new RelayCommand(Register, CanRegister);
                }
                return registerCommand;
            }
        }
        private void Register()
        {
            DatatableAgency = dTAgencys.InsertAgency(AgencyCode, ServerIP, KeyGenerator.SHA384Encode(VerificationCode));
            OnPropertyChanged("Tip");
        }
        private bool CanRegister()
        {
            if (CheckTextBox())
            {
                if (IsAgencyExist)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
        #region 啟用鈕
        private RelayCommand runCommand;
        public ICommand RunCommand
        {
            get
            {
                if (runCommand == null)
                {
                    runCommand = new RelayCommand(RunButton, CanRun);
                }
                return runCommand;
            }
        }
        private bool CanRun()
        {
            if (CheckTextBox())
            {
                if (IsAgencyExist)
                {
                    if ((IsVerify && IsTry) || !IsVerify)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void RunButton()
        {
            DatatableAgency = dTAgencys.UpdateAgencyStatus("RUN", AgencyCode);
            OnPropertyChanged("Tip");
        }
        #endregion
        #region 停用鈕
        private RelayCommand stopCommand;
        public ICommand StopCommand
        {
            get
            {
                if (stopCommand == null)
                {
                    stopCommand = new RelayCommand(StopButton, CanStop);
                }
                return stopCommand;
            }
        }
        private bool CanStop()
        {
            if (CheckTextBox())
            {
                if (IsAgencyExist)
                {
                    if (IsVerify)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void StopButton()
        {
            DatatableAgency = dTAgencys.UpdateAgencyStatus("STOP", AgencyCode);
            OnPropertyChanged("Tip");
        }
        #endregion
        #region 試用鈕
        private RelayCommand tryCommand;
        public ICommand TryCommand
        {
            get
            {
                if (tryCommand == null)
                {
                    tryCommand = new RelayCommand(TryButton, CanTry);
                }
                return tryCommand;
            }
        }
        private bool CanTry()
        {
            if (CheckTextBox())
            {
                if (IsAgencyExist)
                {
                    if (IsVerify && !IsTry)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void TryButton()
        {
            DatatableAgency = dTAgencys.UpdateAgencyStatus("TRY", AgencyCode);
            OnPropertyChanged("Tip");
        }
        #endregion
        #region METHOD
        private bool CheckTextBox()
        {
            if (!string.IsNullOrEmpty(AgencyCode) && !string.IsNullOrEmpty(VerificationCode))
            {
                if (VerificationCode.Equals(KeyGenerator.SHA384Encode(AgencyCode)))
                    return true;
            }
            return false;
        }
        #endregion
    }
}
