using System;
using System.Collections;
using Server.Network;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName( "a fan dancer corpse" )]
	public class FanDancer : BaseCreature
	{
		[Constructable]
		public FanDancer() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a fan dancer";
			Body = 247;
			BaseSoundID = 0x372;

			SetStr( 301, 375 );
			SetDex( 201, 255 );
			SetInt( 21, 25 );

			SetHits( 351, 430 );

			SetDamage( 12, 17 );

			SetSkill( SkillName.MagicResist, 100.1, 110.0 );
			SetSkill( SkillName.Tactics, 85.1, 95.0 );
			SetSkill( SkillName.Wrestling, 85.1, 95.0 );
			SetSkill( SkillName.Anatomy, 85.1, 95.0 );

			Fame = 9000;
			Karma = -9000;
		}
				
				
		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.Gems, 2 );
		}
			
		public override bool Uncalmable{ get{ return true; } }

		public override void OnDamagedBySpell( Mobile attacker )
		{
			base.OnDamagedBySpell( attacker );
			
			if ( 0.8 > Utility.RandomDouble() && !attacker.InRange( this, 1 ) )
			{
				Effects.SendPacket( attacker, attacker.Map, new HuedEffect( EffectType.Moving, Serial.Zero, Serial.Zero, 0x27A3, this.Location, attacker.Location, 10, 0, false, false, 0, 0 ) );
                this.Damage(Utility.RandomMinMax(50, 65), attacker);
            }
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );
			
			if ( 0.8 > Utility.RandomDouble() && !attacker.InRange( this, 1 ) )
			{
				Effects.SendPacket( attacker, attacker.Map, new HuedEffect( EffectType.Moving, Serial.Zero, Serial.Zero, 0x27A3, this.Location, attacker.Location, 10, 0, false, false, 0, 0 ) );
                this.Damage(Utility.RandomMinMax(50, 65), attacker);
            }
		}
		
		public FanDancer( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}