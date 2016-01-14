using System;
using System.Collections;

namespace Server.Items
{
	public abstract class BaseRunicTool : BaseTool
	{
		private CraftResource m_Resource;

		[CommandProperty( AccessLevel.GameMaster )]
		public CraftResource Resource
		{
			get{ return m_Resource; }
			set{ m_Resource = value; Hue = CraftResources.GetHue( m_Resource ); InvalidateProperties(); }
		}

		public BaseRunicTool( CraftResource resource, int itemID ) : base( itemID )
		{
			m_Resource = resource;
		}

		public BaseRunicTool( CraftResource resource, int uses, int itemID ) : base( uses, itemID )
		{
			m_Resource = resource;
		}

		public BaseRunicTool( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
			writer.Write( (int) m_Resource );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Resource = (CraftResource)reader.ReadInt();
					break;
				}
			}
		}

		private static bool m_IsRunicTool;
		private static int m_LuckChance;
		private const int MaxProperties = 32;
		private static BitArray m_Props = new BitArray( MaxProperties );
		private static int[] m_Possible = new int[MaxProperties];

		public static int GetUniqueRandom( int count )
		{
			int avail = 0;

			for ( int i = 0; i < count; ++i )
			{
				if ( !m_Props[i] )
					m_Possible[avail++] = i;
			}

			if ( avail == 0 )
				return -1;

			int v = m_Possible[Utility.Random( avail )];

			m_Props.Set( v, true );

			return v;
		}

		public static void ApplyAttributesTo( BaseWeapon weapon, int attributeCount )
		{
			ApplyAttributesTo( weapon, false, 0, attributeCount );
		}

		public static void ApplyAttributesTo( BaseWeapon weapon, bool isRunicTool, int luckChance, int attributeCount )
		{
			m_IsRunicTool = isRunicTool;
			m_LuckChance = luckChance;

			m_Props.SetAll( false );

			if ( weapon is BaseRanged )
				m_Props.Set( 2, true ); // ranged weapons cannot be ubws or mageweapon

			for ( int i = 0; i < attributeCount; ++i )
			{
				int random = GetUniqueRandom( 25 );

				if ( random == -1 )
					break;

				switch ( random )
				{
					case 23: weapon.Slayer = GetRandomSlayer(); break;
				}
			}
		}

		public static SlayerName GetRandomSlayer()
		{
			// TODO: Check random algorithm on OSI

			SlayerGroup[] groups = SlayerGroup.Groups;

			if ( groups.Length == 0 )
				return SlayerName.None;

			SlayerGroup group = groups[Utility.Random( groups.Length -1 )]; //-1 To Exclude the Fey Slayer which appears ONLY on a certain artifact.
			SlayerEntry entry;

			if ( 10 > Utility.Random( 100 ) ) // 10% chance to do super slayer
			{
				entry = group.Super;
			}
			else
			{
				SlayerEntry[] entries = group.Entries;

				if ( entries.Length == 0 )
					return SlayerName.None;

				entry = entries[Utility.Random( entries.Length )];
			}

			return entry.Name;
		}

		public static void ApplyAttributesTo( BaseArmor armor, int attributeCount)
		{
			ApplyAttributesTo( armor, false, 0, attributeCount );
		}

		public static void ApplyAttributesTo( BaseArmor armor, bool isRunicTool, int luckChance, int attributeCount )
		{
			m_IsRunicTool = isRunicTool;
			m_LuckChance = luckChance;

			m_Props.SetAll( false );

			bool isShield = ( armor is BaseShield );
			int baseCount = ( isShield ? 7 : 20 );
			int baseOffset = ( isShield ? 0 : 4 );

			if ( !isShield && armor.MeditationAllowance == ArmorMeditationAllowance.All )
				m_Props.Set( 3, true ); // remove mage armor from possible properties
			if ( armor.Resource >= CraftResource.RegularLeather && armor.Resource <= CraftResource.BarbedLeather )
			{
				m_Props.Set( 0, true ); // remove lower requirements from possible properties for leather armor
				m_Props.Set( 2, true ); // remove durability bonus from possible properties
			}
			if ( armor.RequiredRace == Race.Elf )
				m_Props.Set( 7, true ); // elves inherently have night sight and elf only armor doesn't get night sight as a mod

			for ( int i = 0; i < attributeCount; ++i )
			{
				int random = GetUniqueRandom( baseCount );

				if ( random == -1 )
					break;

				random += baseOffset;
			}
		}

		public static void ApplyAttributesTo( int attributeCount )
		{
			ApplyAttributesTo( false, 0, attributeCount );
		}

		public static void ApplyAttributesTo( bool isRunicTool, int luckChance, int attributeCount )
		{
			m_IsRunicTool = isRunicTool;
			m_LuckChance = luckChance;

			m_Props.SetAll( false );

			for ( int i = 0; i < attributeCount; ++i )
			{
				int random = GetUniqueRandom( 24 );

				if ( random == -1 )
					break;
			}
		}

		public static void ApplyAttributesTo( Spellbook spellbook, bool isRunicTool, int luckChance, int attributeCount )
		{
			m_IsRunicTool = isRunicTool;
			m_LuckChance = luckChance;

			m_Props.SetAll( false );

			for ( int i = 0; i < attributeCount; ++i )
			{
				int random = GetUniqueRandom( 16 );

				if ( random == -1 )
					break;

				switch ( random )
				{
					case 15: spellbook.Slayer = GetRandomSlayer(); break;
				}
			}
		}
	}
}