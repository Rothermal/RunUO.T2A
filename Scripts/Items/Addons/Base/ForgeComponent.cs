namespace Server.Items
{
    [Server.Engines.Craft.Forge]
    public class ForgeComponent : AddonComponent
    {
        [Constructable]
        public ForgeComponent(int itemID) : base(itemID)
        {
        }

        public ForgeComponent(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}