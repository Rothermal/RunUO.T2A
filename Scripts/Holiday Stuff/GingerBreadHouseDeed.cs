namespace Server.Items
{
    public class GingerBreadHouseAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new GingerBreadHouseDeed(); } }

		public GingerBreadHouseAddon()
		{
			for( int i=0x2be5; i<0x2be8; i++ )
			{
				LocalizedAddonComponent laoc = new LocalizedAddonComponent( i, 1077395 ); // Gingerbread House
				laoc.Light = LightType.SouthSmall;
				AddComponent( laoc, (i==0x2be5) ? -1 : 0, (i==0x2be7) ? -1 : 0, 0 ); 
			}
		}

		public GingerBreadHouseAddon( Serial serial ) : base( serial )
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

	public class GingerBreadHouseDeed : BaseAddonDeed
	{
		public override int LabelNumber{ get{ return 1077394; } } //a Gingerbread House Deed
		public override BaseAddon Addon{ get{ return new GingerBreadHouseAddon(); } }

		[Constructable]
		public GingerBreadHouseDeed() 
		{
			Weight = 1.0;
			LootType = LootType.Blessed;
		}

		public GingerBreadHouseDeed( Serial serial ) : base( serial )
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





    public class LocalizedAddonComponent : AddonComponent
    {
        private int m_LabelNumber;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Number
        {
            get { return m_LabelNumber; }
            set { m_LabelNumber = value; InvalidateProperties(); }
        }

        public override int LabelNumber { get { return m_LabelNumber; } }

        [Constructable]
        public LocalizedAddonComponent(int itemID, int labelNumber) : base(itemID)
        {
            m_LabelNumber = labelNumber;
        }

        public LocalizedAddonComponent(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)m_LabelNumber);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_LabelNumber = reader.ReadInt();
                        break;
                    }
            }
        }
    }




}

