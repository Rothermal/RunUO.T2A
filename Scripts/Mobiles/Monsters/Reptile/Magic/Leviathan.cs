using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName( "a leviathan corpse" )]
	public class Leviathan : BaseCreature
	{
		private Mobile m_Fisher;

		public Mobile Fisher
		{
			get{ return m_Fisher; }
			set{ m_Fisher = value; }
		}

		[Constructable]
		public Leviathan() : this( null )
		{
		}

		[Constructable]
		public Leviathan( Mobile fisher ) : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			m_Fisher = fisher;

			// May not be OSI accurate; mostly copied from krakens
			Name = "a leviathan";
			Body = 77;
			BaseSoundID = 353;

			Hue = 0x481;

			SetStr( 1000 );
			SetDex( 501, 520 );
			SetInt( 501, 515 );

			SetHits( 1500 );

			SetDamage( 25, 33 );

			SetSkill( SkillName.EvalInt, 97.6, 107.5 );
			SetSkill( SkillName.Magery, 97.6, 107.5 );
			SetSkill( SkillName.MagicResist, 97.6, 107.5 );
			SetSkill( SkillName.Meditation, 97.6, 107.5 );
			SetSkill( SkillName.Tactics, 97.6, 107.5 );
			SetSkill( SkillName.Wrestling, 97.6, 107.5 );

			Fame = 24000;
			Karma = -24000;

			VirtualArmor = 50;

			CanSwim = true;
			CantWalk = true;

			PackItem( new MessageInABottle() );

			Rope rope = new Rope();
			rope.ItemID = 0x14F8;
			PackItem( rope );

			rope = new Rope();
			rope.ItemID = 0x14FA;
			PackItem( rope );
		}

		public override bool HasBreath{ get{ return true; } }
		public override int BreathEffectHue{ get{ return 0x1ED; } }
		public override double BreathDamageScalar{ get{ return 0.05; } }
		public override double BreathMinDelay{ get{ return 5.0; } }
		public override double BreathMaxDelay{ get{ return 7.5; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 5 );
		}

		public override double TreasureMapChance{ get{ return 0.25; } }
		public override int TreasureMapLevel{ get{ return 5; } }

		public Leviathan( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}