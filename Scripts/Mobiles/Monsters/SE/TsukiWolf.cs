using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName( "a tsuki wolf corpse" )]
	public class TsukiWolf : BaseCreature
	{
		[Constructable]
		public TsukiWolf()
			: base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a tsuki wolf";
			Body = 250;

			switch( Utility.Random( 3 ) )
			{
				case 0: Hue = Utility.RandomNeutralHue(); break; //No, this really isn't accurate ;->
			}

			SetStr( 401, 450 );
			SetDex( 151, 200 );
			SetInt( 66, 76 );

			SetHits( 376, 450 );
			SetMana( 40 );

			SetDamage( 14, 18 );

			SetSkill( SkillName.Anatomy, 65.1, 72.0 );
			SetSkill( SkillName.MagicResist, 65.1, 70.0 );
			SetSkill( SkillName.Tactics, 95.1, 110.0 );
			SetSkill( SkillName.Wrestling, 97.6, 107.5 );

			Fame = 8500;
			Karma = -8500;

			switch( Utility.Random( 10 ) )
			{
				case 0: PackItem( new LeftArm() ); break;
				case 1: PackItem( new RightArm() ); break;
				case 2: PackItem( new Torso() ); break;
				case 3: PackItem( new Bone() ); break;
				case 4: PackItem( new RibCage() ); break;
				case 5: PackItem( new RibCage() ); break;
				case 6: PackItem( new BonePile() ); break;
				case 7: PackItem( new BonePile() ); break;
				case 8: PackItem( new BonePile() ); break;
				case 9: PackItem( new BonePile() ); break;
			}
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Rich );
		}
		public override int Meat { get { return 4; } }
		public override int Hides { get { return 25; } }
		public override FoodType FavoriteFood { get { return FoodType.Meat; } }

		public TsukiWolf( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override int GetAngerSound()
		{
			return 0x52D;
		}

		public override int GetIdleSound()
		{
			return 0x52C;
		}

		public override int GetAttackSound()
		{
			return 0x52B;
		}

		public override int GetHurtSound()
		{
			return 0x52E;
		}

		public override int GetDeathSound()
		{
			return 0x52A;
		}
	}
}