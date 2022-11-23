using System;
using System.Drawing;
using System.Windows.Forms;

namespace ApiChecker.Ucs
{
    public partial class ApiStatusUc : UserControl
    {
        private readonly ApiCheck _apiCheck;
        public Guid Id { get; }
        private string _apiName;
        public string Address;

        public ApiStatusUc(Guid id, ApiCheck apiCheck)
        {
            InitializeComponent();

            _apiCheck = apiCheck;
            Id = id;
            UpdateName(_apiCheck.Name);
            UpdateAddress(_apiCheck.Address);
        }

        private void UpdateName(string name)
        {
            _apiName = name;
            lblName.Text = $"{_apiName}";
        }

        private void UpdateAddress(string address)
        {
            Address = address;
            lblAddress.Text = $"{Address}";
        }

        public void UpdateApiStatus()
        {
            lblLastCheck.Text = _apiCheck.LastCheck.ToString("MM/dd hh:mm:ss");
            lblLastSuccessTime.Text = _apiCheck.LastSuccessTime.ToString("MM/dd hh:mm:ss");
            if (_apiCheck.IsRunning)
            {
                lblStatus.Text = "Ok";
                lblStatus.ForeColor = Color.Green;
            }
            else
            {

                lblStatus.Text = "Failed";
                lblStatus.ForeColor = Color.Red;
            }
        }
    }
}
