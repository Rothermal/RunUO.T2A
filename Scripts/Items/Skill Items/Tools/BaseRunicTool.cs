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

		private static int Scale( int min, int max, int low, int high )
		{
			int percent;

			if ( m_IsRunicTool )
			{
				percent = Utility.RandomMinMax( min, max );
			}
			else
			{
				// Behold, the worst system ever!
				int v = Utility.RandomMinMax( 0, 10000 );

				v = (int) Math.Sqrt( v );
				v = 100 - v;

				if ( LootPack.CheckLuck( m_LuckChance ) )
					v += 10;

				if ( v < min )
					v = min;
				else if ( v > max )
					v = max;

				percent = v;
			}

			int scaledBy = Math.Abs( high - low ) + 1;

			if ( scaledBy != 0 )
				scaledBy = 10000 / scaledBy;

			percent *= (10000 + scaledBy);

			return low + (((high - low) * percent) / 1000001);
		}


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

		public void ApplyAttributesTo( BaseWeapon weapon )
		{
			CraftResourceInfo resInfo = CraftResources.GetInfo( m_Resource );

			if ( resInfo == null )
				return;

			CraftAttributeInfo attrs = resInfo.AttributeInfo;

			if ( attrs == null )
				return;

			int attributeCount = Utility.RandomMinMax( attrs.RunicMinAttributes, attrs.RunicMaxAttributes );
			int min = attrs.RunicMinIntensity;
			int max = attrs.RunicMaxIntensity;

			ApplyAttributesTo( weapon, true, 0, attributeCount, min, max );
		}

		public static void ApplyAttributesTo( BaseWeapon weapon, int attributeCount, int min, int max )
		{
			ApplyAttributesTo( weapon, false, 0, attributeCount, min, max );
		}

		public static void ApplyAttributesTo( BaseWeapon weapon, bool isRunicTool, int luckChance, int attributeCount, int min, int max )
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

		public void ApplyAttributesTo( BaseArmor armor )
		{
			CraftResourceInfo resInfo = CraftResources.GetInfo( m_Resource );

			if ( resInfo == null )
				return;

			CraftAttributeInfo attrs = resInfo.AttributeInfo;

			if ( attrs == null )
				return;

			int attributeCount = Utility.RandomMinMax( attrs.RunicMinAttributes, attrs.RunicMaxAttributes );
			int min = attrs.RunicMinIntensity;
			int max = attrs.RunicMaxIntensity;

			ApplyAttributesTo( armor, true, 0, attributeCount, min, max );
		}

		public static void ApplyAttributesTo( BaseArmor armor, int attributeCount, int min, int max )
		{
			ApplyAttributesTo( armor, false, 0, attributeCount, min, max );
		}

		public static void ApplyAttributesTo( BaseArmor armor, bool isRunicTool, int luckChance, int attributeCount, int min, int max )
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

		public static void ApplyAttributesTo( BaseHat hat, int attributeCount, int min, int max )
		{
			ApplyAttributesTo( hat, false, 0, attributeCount, min, max );
		}

		public static void ApplyAttributesTo( BaseHat hat, bool isRunicTool, int luckChance, int attributeCount, int min, int max )
		{
			m_IsRunicTool = isRunicTool;
			m_LuckChance = luckChance;

			m_Props.SetAll( false );
		}

		public static void ApplyAttributesTo( BaseJewel jewelry, int attributeCount, int min, int max )
		{
			ApplyAttributesTo( jewelry, false, 0, attributeCount, min, max );
		}

		public static void ApplyAttributesTo( BaseJewel jewelry, bool isRunicTool, int luckChance, int attributeCount, int min, int max )
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

		public static void ApplyAttributesTo( Spellbook spellbook, int attributeCount, int min, int max )
		{
			ApplyAttributesTo( spellbook, false, 0, attributeCount, min, max );
		}

		public static void ApplyAttributesTo( Spellbook spellbook, bool isRunicTool, int luckChance, int attributeCount, int min, int max )
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