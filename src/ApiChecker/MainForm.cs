using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ApiChecker.Ucs;
using Serilog;

namespace ApiChecker
{
    public partial class MainForm : Form
    {
        private readonly AppConfig _appConfig;
        private List<Tuple<Guid, ApiCheck, ApiStatusUc>> _apiStatusUcs;

        public MainForm(AppConfig appConfig)
        {
            _appConfig = appConfig;
            InitializeComponent();

            Load += async (_, b) => await ReadApiAddresses();
        }

        private async Task ReadApiAddresses()
        {
            try
            {
                lblInterval.Text = _appConfig.IntervalInSeconds.ToString();
                _apiStatusUcs = new List<Tuple<Guid, ApiCheck, ApiStatusUc>>();
                foreach (var apiAddress in _appConfig.ApiAddresses.Where(a => a.Enabled))
                {
                    var id = Guid.NewGuid();
                    var apiCheck = new ApiCheck(id, apiAddress.Name, apiAddress.Address);
                    var uc = new ApiStatusUc(id, apiCheck);
                    flowLayoutPanel1.Controls.Add(uc);
                    _apiStatusUcs.Add(new Tuple<Guid, ApiCheck, ApiStatusUc>(id, apiCheck, uc));
                }

                await Execute(DoUpdateStatus, 0);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in ReadApiAddresses");
                throw;
            }
        }

        private async void DoUpdateStatus()
        {
            await UpdateStatus();
        }

        private async Task UpdateStatus()
        {
            foreach (var apiStatusUc in _apiStatusUcs)
            {
                await apiStatusUc.Item2.Check();
                apiStatusUc.Item3.UpdateApiStatus();
            }

            lblLastCheck.Text = DateTime.Now.ToString("MM/dd hh:mm:ss");
            await Execute(Action, _appConfig.IntervalInSeconds * 1000);
        }

        private async void Action()
        {
            await UpdateStatus();
        }

        private async Task Execute(Action action, int timeoutInMilliseconds)
        {
            await Task.Delay(timeoutInMilliseconds);
            action();
        }
    }
}