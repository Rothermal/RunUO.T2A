using System;
using Server.Items;
using Server.Network;
using System.Collections.Generic;

namespace Server.Mobiles
{
    [CorpseName("an interred grizzle corpse")]
	public class InterredGrizzle  : BaseCreature
	{
		[Constructable]
		public  InterredGrizzle () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an interred grizzle";
			Body = 259;

			SetStr( 451, 500 );
			SetDex( 201, 250 );
			SetInt( 801, 850 );

			SetHits( 1500 );
			SetStam( 150 );

			SetDamage( 16, 19 );

			SetSkill(SkillName.Meditation, 77.7, 84.0 );
			SetSkill(SkillName.EvalInt, 72.2, 79.6 );
			SetSkill(SkillName.Magery, 83.7, 89.6);
			SetSkill(SkillName.Poisoning, 0 );
			SetSkill(SkillName.Anatomy, 0 );
			SetSkill( SkillName.MagicResist, 80.2, 87.3 );
			SetSkill( SkillName.Tactics, 104.5, 105.1 );
			SetSkill( SkillName.Wrestling, 105.1, 109.4 );

			Fame = 3700;  // Guessed
			Karma = -3700;  // Guessed
		}

		public override void GenerateLoot() // -- Need to verify
		{
			AddLoot( LootPack.FilthyRich );
		}

		// TODO: Acid Blood
		/*
		 * Message: 1070820
		 * Spits pool of acid (blood, hue 0x3F), hits lost 6-10 per second/step
		 * Damage is resistable (physical)
		 * Acid last 10 seconds
		 */
		 
		public override int GetAngerSound()
		{
			return 0x581;
		}

		public override int GetIdleSound()
		{
			return 0x582;
		}

		public override int GetAttackSound()
		{
			return 0x580;
		}

		public override int GetHurtSound()
		{
			return 0x583;
		}

		public override int GetDeathSound()
		{
			return 0x584;
		}

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			if( Utility.RandomDouble() < 0.1 )
				DropOoze();

			base.OnDamage( amount, from, willKill );
		}

		private int RandomPoint( int mid )
		{
			return ( mid + Utility.RandomMinMax( -2, 2 ) );
		}

		public virtual Point3D GetSpawnPosition( int range )
		{
			return GetSpawnPosition( Location, Map, range );
		}

		public virtual Point3D GetSpawnPosition( Point3D from, Map map, int range )
		{
			if( map == null )
				return from;

			Point3D loc = new Point3D( ( RandomPoint( X ) ), ( RandomPoint( Y ) ), Z );

			loc.Z = Map.GetAverageZ( loc.X, loc.Y );

			return loc;
		}

		public virtual void DropOoze()
		{
			int amount = Utility.RandomMinMax( 1, 3 );
			bool corrosive = Utility.RandomBool();

			for( int i = 0; i < amount; i++ )
			{
				Item ooze = new StainedOoze( corrosive );
				Point3D p = new Point3D( Location );

				for( int j = 0; j < 5; j++ )
				{
					p = GetSpawnPosition( 2 );
					bool found = false;

					foreach( Item item in Map.GetItemsInRange( p, 0 ) )
						if( item is StainedOoze )
						{
							found = true;
							break;
						}

					if( !found )
						break;
				}

				ooze.MoveToWorld( p, Map );
			}

			if( Combatant != null )
			{
				if( corrosive )
					Combatant.SendLocalizedMessage( 1072071 ); // A corrosive gas seeps out of your enemy's skin!
				else
					Combatant.SendLocalizedMessage( 1072072 ); // A poisonous gas seeps out of your enemy's skin!
			}
		}

		public  InterredGrizzle ( Serial serial ) : base( serial )
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

    public class StainedOoze : Item
    {
        private bool m_Corrosive;
        private Timer m_Timer;
        private int m_Ticks;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Corrosive
        {
            get { return m_Corrosive; }
            set { m_Corrosive = value; }
        }

        [Constructable]
        public StainedOoze()
            : this(false)
        {
        }

        [Constructable]
        public StainedOoze(bool corrosive)
            : base(0x122A)
        {
            Movable = false;
            Hue = 0x95;

            m_Corrosive = corrosive;
            m_Timer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1), OnTick);
            m_Ticks = 0;
        }

        public override void OnAfterDelete()
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }
        }

        private void OnTick()
        {
            List<Mobile> toDamage = new List<Mobile>();

            foreach (Mobile m in GetMobilesInRange(0))
            {
                if (m is BaseCreature)
                {
                    BaseCreature bc = (BaseCreature)m;

                    if (!bc.Controlled && !bc.Summoned)
                        continue;
                }
                else if (!m.Player)
                {
                    continue;
                }

                if (m.Alive && !m.IsDeadBondedPet && m.CanBeDamaged())
                    toDamage.Add(m);
            }

            for (int i = 0; i < toDamage.Count; ++i)
                Damage(toDamage[i]);

            ++m_Ticks;

            if (m_Ticks >= 35)
                Delete();
            else if (m_Ticks == 30)
                ItemID = 0x122B;
        }

        public void Damage(Mobile m)
        {
            if (m_Corrosive)
            {
                List<Item> items = m.Items;
                bool damaged = false;

                for (int i = 0; i < items.Count; ++i)
                {
                    IDurability wearable = items[i] as IDurability;

                    if (wearable != null && wearable.HitPoints >= 10 && Utility.RandomDouble() < 0.25)
                    {
                        wearable.HitPoints -= (wearable.HitPoints == 10) ? Utility.Random(1, 5) : 10;
                        damaged = true;
                    }
                }

                if (damaged)
                {
                    m.LocalOverheadMessage(MessageType.Regular, 0x21, 1072070); // The infernal ooze scorches you, setting you and your equipment ablaze!
                    return;
                }
            }

            m.Damage(40);
        }

        public StainedOoze(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(m_Corrosive);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Corrosive = reader.ReadBool();

            m_Timer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1), OnTick);
            m_Ticks = (ItemID == 0x122A) ? 0 : 30;
        }
    }
}
