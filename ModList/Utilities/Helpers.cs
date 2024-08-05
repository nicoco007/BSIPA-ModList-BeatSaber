using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Tags;
using HMUI;
using IPA.Loader;
using IPA.Utilities.Async;
using TMPro;
using UnityEngine;
using BSMLUtils = BeatSaberMarkupLanguage.Utilities;

namespace IPA.ModList.BeatSaber.Utilities
{
    internal static class Helpers
    {
        private const string ResourcePrefix = "IPA.ModList.BeatSaber.Resources.";

        public static Sprite? DefaultPluginIcon { get; private set; }

        public static Sprite? LibraryIcon { get; private set; }

        public static Sprite? XSprite  { get; private set; }

        public static Sprite? OSprite { get; private set; }

        public static Sprite? WarnSprite { get; private set; }

        public static Sprite? SmallRoundedRectSprite { get; private set; }

        public static Sprite? TinyRoundedRectSprite { get; private set; }

        public static HMUI.Screen MainScreen => BeatSaberUI.DiContainer.Resolve<HierarchyManager>()._screenSystem.mainScreen;

        public static TextTag TextTag { get; } = new TextTag();

        public static async Task LoadResourcesAsync(Assembly assembly)
        {
            DefaultPluginIcon = await BSMLUtils.LoadSpriteFromAssemblyAsync(assembly, ResourcePrefix + "plugin.png");
            LibraryIcon = await BSMLUtils.LoadSpriteFromAssemblyAsync(assembly, ResourcePrefix + "library.png");
            XSprite = await BSMLUtils.LoadSpriteFromAssemblyAsync(assembly, ResourcePrefix + "x.png");
            OSprite = await BSMLUtils.LoadSpriteFromAssemblyAsync(assembly, ResourcePrefix + "o.png");
            WarnSprite = await BSMLUtils.LoadSpriteFromAssemblyAsync(assembly, ResourcePrefix + "!.png");
            SmallRoundedRectSprite = await LoadSmallRoundedRectSprite(assembly);
            TinyRoundedRectSprite = await LoadTinyRoundedRectSprite(assembly);
        }

        private static async Task<Sprite> LoadSmallRoundedRectSprite(Assembly assembly)
        {
            var tex = await BSMLUtils.LoadTextureFromAssemblyAsync(assembly, ResourcePrefix + "small-rounded-rect.png")!;
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),
                pivot: Vector2.zero,
                border: new Vector4(32, 32, 32, 32),
                pixelsPerUnit: 100f,
                extrude: 0,
                meshType: SpriteMeshType.FullRect);
        }

        private static async Task<Sprite> LoadTinyRoundedRectSprite(Assembly assembly)
        {
            var tex = await BSMLUtils.LoadTextureFromAssemblyAsync(assembly, ResourcePrefix + "tiny-rounded-rect.png")!;
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),
                pivot: Vector2.zero,
                border: new Vector4(8, 8, 8, 8),
                pixelsPerUnit: 100f,
                extrude: 0,
                meshType: SpriteMeshType.FullRect);
        }

        public static Sprite? QueueReadPluginIcon(this PluginMetadata plugin, Action<Sprite?> onCompletion)
        {
            if (plugin.IsBare)
            {
                return LibraryIcon;
            }

            UnityMainThreadTaskScheduler.Factory.StartNew(async () =>
            {
                Sprite? icon = null;

                if (plugin.IconName != null)
                {
                    icon = await BSMLUtils.LoadSpriteFromAssemblyAsync(plugin.Assembly, plugin.IconName);
                }

                onCompletion.Invoke(icon != null ? icon : DefaultPluginIcon);
            }).ContinueWith((task) => Plugin.Logger?.Error(task.Exception), TaskContinuationOptions.OnlyOnFaulted);

            return BSMLUtils.ImageResources.BlankSprite;
        }

        public static CurvedTextMeshPro CreateText(string text, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            var gameObj = TextTag.CreateObject(MainScreen.transform);
            var tmp = gameObj.GetComponent<FormattableText>();
            tmp.text = text;
            tmp.rectTransform.sizeDelta = sizeDelta;
            tmp.rectTransform.anchoredPosition = anchoredPosition;
            return tmp;
        }

        public static TMP_FontAsset TMPFontFromUnityFont(Font font)
        {
            var asset = TMP_FontAsset.CreateFontAsset(font);
            asset.name = font.name;
            asset.hashCode = TMP_TextUtilities.GetSimpleHashCode(asset.name);
            return asset;
        }

        public static IEnumerable<T> SingleEnumerable<T>(T item)
        {
            yield return item;
        }

        public static void Zero(RectTransform transform)
        {
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.anchoredPosition = Vector2.zero;
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            transform.sizeDelta = Vector2.zero;
        }

        public static IEnumerable<T> AppendIf<T>(this IEnumerable<T> enumerable, bool append, T item) => append ? enumerable.Append(item) : enumerable;
    }
}