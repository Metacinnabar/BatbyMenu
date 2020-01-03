using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace BatbyMenu
{
	public class BatbyMenu : Mod
	{
		//Icon.. white with border
		//Github
		//Draw over bottom text bottom left corner
		//Use library for blank texture & overhaul mod, load backgrounds, mainmenu background swap

		internal Texture2D vanillaFrontMainMenuBackground;
		internal Texture2D vanillaMiddleMainMenuBackground;
		internal Texture2D vanillaBackMainMenuBackground;
		internal Texture2D vanillaSkyBackground;
		internal Texture2D vanillaLogoDay;
		internal Texture2D vanillaLogoNight;
		internal Texture2D[] vanillaCloud = new Texture2D[22];
		private bool titleChosen;
		private bool unloadCalled;

		public override void Load()
		{
			unloadCalled = false;

			vanillaFrontMainMenuBackground = Main.backgroundTexture[173];
			vanillaMiddleMainMenuBackground = Main.backgroundTexture[172];
			vanillaBackMainMenuBackground = Main.backgroundTexture[171];
			vanillaSkyBackground = Main.backgroundTexture[0];
			vanillaLogoDay = Main.logoTexture;
			vanillaLogoNight = Main.logo2Texture;
			for (int i = 0; i < vanillaCloud.Length; i++)
			{
				vanillaCloud[i] = Main.cloudTexture[i];
			}

			Main.OnTick += OnUpdate;
		}

		public override void Unload()
		{
			unloadCalled = true;

			Main.backgroundTexture[173] = vanillaFrontMainMenuBackground;
			Main.backgroundTexture[172] = vanillaMiddleMainMenuBackground;
			Main.backgroundTexture[171] = vanillaBackMainMenuBackground;
			Main.backgroundTexture[0] = vanillaSkyBackground;
			Main.logoTexture = vanillaLogoDay;
			Main.logo2Texture = vanillaLogoNight;
			for (int i = 0; i < vanillaCloud.Length; i++)
			{
				Main.cloudTexture[i] = vanillaCloud[i];
			}

			Main.OnTick -= OnUpdate;
		}

		private void OnUpdate()
		{
			if (Main.gameMenu)
			{
				Mod Overhaul = ModLoader.GetMod("TerrariaOverhaul");
				if (Overhaul == null)
				{
					if (ModContent.GetInstance<MConfig>().WhiteMenu)
					{
						Main.dayTime = true;
						Main.time = 27000.0;
						Main.sunModY = 64;

						if (!Main.dedServ)
						{
							LoadBackgrounds(new List<int>() { 173, 171, 172, 0 });
							MainMenuBackgroundReplaceAll(GetTexture("Sky"));
							for (int vanillaCloudTextureID = 0; vanillaCloudTextureID < vanillaCloud.Length; vanillaCloudTextureID++)
							{
								Main.cloudTexture[vanillaCloudTextureID] = GetTexture("Blank");
							}
							Main.backgroundTexture[0] = GetTexture("Sky");
							Main.logoTexture = Main.logo2Texture = GetTexture("Blank");
						}
					}
					else
					{
						if (!Main.dedServ)
						{
							Main.backgroundTexture[173] = vanillaFrontMainMenuBackground;
							Main.backgroundTexture[172] = vanillaMiddleMainMenuBackground;
							Main.backgroundTexture[171] = vanillaBackMainMenuBackground;
							Main.backgroundTexture[0] = vanillaSkyBackground;
							Main.logoTexture = vanillaLogoDay;
							Main.logo2Texture = vanillaLogoNight;
							for (int i = 0; i < vanillaCloud.Length; i++)
							{
								Main.cloudTexture[i] = vanillaCloud[i];
							}
						}
					}
					if (Main.menuMode == 10006 && !unloadCalled)
					{
						Unload();
						return;
					}
				}
			}
		}

		public override void PreSaveAndQuit()
		{
			//Sets the main menu back to the original main menu background
			WorldGen.setBG(0, 6);
		}

		public override void UpdateMusic(ref int music, ref MusicPriority priority)
		{
			//Blank game title
			if (titleChosen == false)
			{
				GameWindow windowValue = (GameWindow)typeof(Game).GetProperty("Window", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).GetValue(Main.instance);
				GameWindow gameWindow = Main.instance.Window ?? windowValue;
				if (gameWindow != null)
					gameWindow.Title = "";
				titleChosen = true;
			}
		}

		internal void MainMenuBackgroundReplaceAll(Texture2D texture)
		{
			Main.backgroundTexture[173] = texture;
			Main.backgroundTexture[172] = texture;
			Main.backgroundTexture[171] = texture;
		}

		internal void LoadBackgrounds(List<int> loadbgNumbers)
		{
			if (!Main.dedServ)
			{
				foreach (int loadbgNumber in loadbgNumbers)
				{
					Main.instance.LoadBackground(loadbgNumber);
				}
			}
		}
	}

	public class MPlayer : ModPlayer
	{
		public override void OnEnterWorld(Player player)
		{
			BackgroundReReplacing(173, 172, 171);
		}

		internal void BackgroundReReplacing(int front, int middle, int back)
		{
			if (!Main.dedServ)
			{
				for (int i = 0; i < ModContent.GetInstance<BatbyMenu>().vanillaCloud.Length; i++)
				{
					Main.cloudTexture[i] = ModContent.GetInstance<BatbyMenu>().vanillaCloud[i];
				}
				Main.backgroundTexture[front] = ModContent.GetInstance<BatbyMenu>().vanillaFrontMainMenuBackground;
				Main.backgroundTexture[middle] = ModContent.GetInstance<BatbyMenu>().vanillaMiddleMainMenuBackground;
				Main.backgroundTexture[back] = ModContent.GetInstance<BatbyMenu>().vanillaBackMainMenuBackground;
			}
		}
	}

	public class MConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		[Header("Main Menu")]
		[Label("Background Changes")]
		[Tooltip("Changes for the main menu. False for no changes, true to change the whole main menu to be white. True by default")]
		[DefaultValue(true)]
		public bool WhiteMenu;
	}
}