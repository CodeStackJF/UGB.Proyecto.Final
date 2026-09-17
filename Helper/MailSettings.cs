using Microsoft.Extensions.Options;

namespace UGB.Proyecto.FinalHelper
{
    public class MailSettings : IConfigureOptions<SettingsBase>
    {
        private readonly IConfiguration conf;
        public MailSettings(IConfiguration _conf)
        {
            conf = _conf;
        }

        public void Configure(SettingsBase options)
        {
            conf.GetSection("MailSettings").Bind(options);
        }
    }

    public class SettingsBase
    {
        public required string Mail { get; set; }
        public required string Password { get; set; }
        public required string DisplayName { get; set; }
        public required string SMTP { get; set; }
        public required int Port { get; set; }
    }
}