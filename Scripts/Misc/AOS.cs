using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.Seventh;
using Server.Spells.Ninjitsu;

namespace Server
{
    public class AOS
	{
		public static void DisableStatInfluences()
		{
			for( int i = 0; i < SkillInfo.Table.Length; ++i )
			{
				SkillInfo info = SkillInfo.Table[i];

				info.StrScale = 0.0;
				info.DexScale = 0.0;
				info.IntScale = 0.0;
				info.StatTotal = 0.0;
			}
		}

		public static int Damage( Mobile m, int damage, bool ignoreArmor, int phys, int fire, int cold, int pois, int nrgy )
		{
			return Damage( m, null, damage, ignoreArmor, phys, fire, cold, pois, nrgy );
		}

		public static int Damage( Mobile m, int damage, int phys, int fire, int cold, int pois, int nrgy )
		{
			return Damage( m, null, damage, phys, fire, cold, pois, nrgy );
		}

		public static int Damage( Mobile m, Mobile from, int damage, int phys, int fire, int cold, int pois, int nrgy )
		{
			return Damage( m, from, damage, false, phys, fire, cold, pois, nrgy, 0, 0, false, false, false );
		}

		public static int Damage( Mobile m, Mobile from, int damage, int phys, int fire, int cold, int pois, int nrgy, int chaos )
		{
			return Damage( m, from, damage, false, phys, fire, cold, pois, nrgy, chaos, 0, false, false, false );
		}

		public static int Damage( Mobile m, Mobile from, int damage, bool ignoreArmor, int phys, int fire, int cold, int pois, int nrgy )
		{
			return Damage( m, from, damage, ignoreArmor, phys, fire, cold, pois, nrgy, 0, 0, false, false, false );
		}

		public static int Damage( Mobile m, Mobile from, int damage, int phys, int fire, int cold, int pois, int nrgy, bool keepAlive )
		{
			return Damage( m, from, damage, false, phys, fire, cold, pois, nrgy, 0, 0, keepAlive, false, false );
		}

		public static int Damage( Mobile m, Mobile from, int damage, bool ignoreArmor, int phys, int fire, int cold, int pois, int nrgy, int chaos, int direct, bool keepAlive, bool archer, bool deathStrike )
		{
			if( m == null || m.Deleted || !m.Alive || damage <= 0 )
				return 0;

			if( phys == 0 && fire == 100 && cold == 0 && pois == 0 && nrgy == 0 )
				Mobiles.MeerMage.StopEffect( m, true );

			m.Damage( damage, from );
			return damage;
		}

		public static void Fix( ref int val )
		{
			if( val < 0 )
				val = 0;
		}

		public static int Scale( int input, int percent )
		{
			return (input * percent) / 100;
		}

		public static int GetStatus( Mobile from, int index )
		{
			switch ( index )
			{
				default: return 0;
			}
		}
	}

	[PropertyObject]
	public abstract class BaseAttributes
	{
		private Item m_Owner;
		private uint m_Names;
		private int[] m_Values;

		private static int[] m_Empty = new int[0];

		public bool IsEmpty { get { return (m_Names == 0); } }
		public Item Owner { get { return m_Owner; } }

		public BaseAttributes( Item owner )
		{
			m_Owner = owner;
			m_Values = m_Empty;
		}

		public BaseAttributes( Item owner, BaseAttributes other )
		{
			m_Owner = owner;
			m_Values = new int[other.m_Values.Length];
			other.m_Values.CopyTo( m_Values, 0 );
			m_Names = other.m_Names;
		}

		public BaseAttributes( Item owner, GenericReader reader )
		{
			m_Owner = owner;

			int version = reader.ReadByte();

			switch( version )
			{
				case 1:
				{
					m_Names = reader.ReadUInt();
					m_Values = new int[reader.ReadEncodedInt()];

					for( int i = 0; i < m_Values.Length; ++i )
						m_Values[i] = reader.ReadEncodedInt();

					break;
				}
				case 0:
				{
					m_Names = reader.ReadUInt();
					m_Values = new int[reader.ReadInt()];

					for( int i = 0; i < m_Values.Length; ++i )
						m_Values[i] = reader.ReadInt();

					break;
				}
			}
		}

		public void Serialize( GenericWriter writer )
		{
			writer.Write( (byte)1 ); // version;

			writer.Write( (uint)m_Names );
			writer.WriteEncodedInt( (int)m_Values.Length );

			for( int i = 0; i < m_Values.Length; ++i )
				writer.WriteEncodedInt( (int)m_Values[i] );
		}

		public int GetValue( int bitmask )
		{
			return 0;
		}

		public void SetValue( int bitmask, int value )
		{
			uint mask = (uint)bitmask;

			if( value != 0 )
			{
				if( (m_Names & mask) != 0 )
				{
					int index = GetIndex( mask );

					if( index >= 0 && index < m_Values.Length )
						m_Values[index] = value;
				}
				else
				{
					int index = GetIndex( mask );

					if( index >= 0 && index <= m_Values.Length )
					{
						int[] old = m_Values;
						m_Values = new int[old.Length + 1];

						for( int i = 0; i < index; ++i )
							m_Values[i] = old[i];

						m_Values[index] = value;

						for( int i = index; i < old.Length; ++i )
							m_Values[i + 1] = old[i];

						m_Names |= mask;
					}
				}
			}
			else if( (m_Names & mask) != 0 )
			{
				int index = GetIndex( mask );

				if( index >= 0 && index < m_Values.Length )
				{
					m_Names &= ~mask;

					if( m_Values.Length == 1 )
					{
						m_Values = m_Empty;
					}
					else
					{
						int[] old = m_Values;
						m_Values = new int[old.Length - 1];

						for( int i = 0; i < index; ++i )
							m_Values[i] = old[i];

						for( int i = index + 1; i < old.Length; ++i )
							m_Values[i - 1] = old[i];
					}
				}
			}

			if( m_Owner.Parent is Mobile )
			{
				Mobile m = (Mobile)m_Owner.Parent;

				m.CheckStatTimers();
				m.Delta( MobileDelta.Stat | MobileDelta.WeaponDamage | MobileDelta.Hits | MobileDelta.Stam | MobileDelta.Mana );
			}

			m_Owner.InvalidateProperties();
		}

		private int GetIndex( uint mask )
		{
			int index = 0;
			uint ourNames = m_Names;
			uint currentBit = 1;

			while( currentBit != mask )
			{
				if( (ourNames & currentBit) != 0 )
					++index;

				if( currentBit == 0x80000000 )
					return -1;

				currentBit <<= 1;
			}

			return index;
		}
	}
}