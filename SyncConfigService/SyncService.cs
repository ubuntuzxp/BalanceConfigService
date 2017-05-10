using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Windows.Forms;

namespace SyncConfigService
{
    public partial class SyncService : ServiceBase
    {
        private Logger log = LogManager.GetCurrentClassLogger();
        //时间间隔180s
        private System.Timers.Timer timer = new System.Timers.Timer(180000);
        private string ConfigDirectory = ConfigurationManager.AppSettings["ConfigDirectory"];
        private string BaseDirectory = ConfigurationManager.AppSettings["BaseDirectory"];
        public SyncService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            log.Info("服务启动");
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.AutoReset = true;
            timer.Enabled = true;
            try
            {
                CopyDllToSystem();
                var init = ConfigBusiness.AclasSDK_Initialize();
                log.Info("AclasSDK_Initialize初始化： " + init);
                ConfigBusiness.SyncConfig();
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace + e.Message);
            }
        }

        protected override void OnStop()
        {
            ConfigBusiness.AclasSDK_Finalize();
            log.Info("服务停止!");
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                ConfigBusiness.SyncConfig();
            }
            catch (Exception ex)
            {
                log.Error(ex.StackTrace + ex.Message);
            }
        }

        private void CopyDllToSystem()
        {
            var filePath = $@"{BaseDirectory}\AclasSDK.dll";
            if (File.Exists(filePath))
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    File.Copy(filePath, @"C:\Windows\SysWOW64\AclasSDK.dll", true);
                }
                else
                {
                    File.Copy(filePath, @"C:\Windows\System32\AclasSDK.dll", true);
                }
                log.Info(filePath + " AclasSDK.dll拷贝成功");
            }
            else
            {
                log.Error(filePath + ":文件不存在");
                Stop();
            }
        }
    }
}
