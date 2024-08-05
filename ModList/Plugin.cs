using System.Reflection;
using System.Threading.Tasks;
using IPA.Config.Stores;
using IPA.Logging;
using IPA.ModList.BeatSaber.Installers;
using IPA.ModList.BeatSaber.Utilities;
using SiraUtil.Zenject;

namespace IPA.ModList.BeatSaber
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static Logger? Logger { get; private set; }

        [Init]
        public void Init(Logger log, Config.Config config, Zenjector zenject)
        {
            Logger = log;
            ModListConfig.Instance ??= config.Generated<ModListConfig>();

            zenject.UseLogger(log);
            zenject.UseMetadataBinder<Plugin>();

            zenject.Install<MLAppInstaller>(Location.App, ModListConfig.Instance);
            zenject.Install<MLMenuInstaller>(Location.Menu);
        }

        [OnEnable]
        public void OnEnable()
        {
            Helpers.LoadResourcesAsync(Assembly.GetExecutingAssembly()).ContinueWith((task) => Logger?.Error(task.Exception), TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}