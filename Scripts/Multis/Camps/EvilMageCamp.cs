using System;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Multis
{
    public class EvilMageCamp  : BaseCamp
    {
        private Mobile m_Prisoner;
 
        [Constructable]
        public EvilMageCamp() : base(0x10EE)
        {
        }

        public override void AddComponents()
        {
            Visible = false;
            DecayDelay = TimeSpan.FromMinutes(5.0);


            AddItem(new Static(4628), 2, 3, 0); // pentagram
            AddItem(new Static(4629), 2, 2, 0); // pentagram
            AddItem(new Static(4622), 2, 1, 0); // pentagram
            AddItem(new Static(4627), 3, 3, 0); // pentagram
            AddItem(new Static(4630), 3, 2, 0); // pentagram
            AddItem(new Static(4623), 3, 1, 0); // pentagram
            AddItem(new Static(4626), 4, 3, 0); // pentagram
            AddItem(new Static(4625), 4, 2, 0); // pentagram
            AddItem(new Static(4624), 4, 1, 0); // pentagram

            AddItem(new Static(4073), 1, 1, 0); // pentagram
            AddItem(new Static(4070), 1, 0, 0); // pentagram
            AddItem(new Static(4071), 1, -1, 0); // pentagram
            AddItem(new Static(4076), 2, 1, 0); // pentagram
            AddItem(new Static(4074), 2, 0, 0); // pentagram
            AddItem(new Static(4072), 2, -1, 0); // pentagram
            AddItem(new Static(4077), 3, 1, 0); // pentagram
            AddItem(new Static(4078), 3, 0, 0); // pentagram
            AddItem(new Static(4075), 3, -1, 0); // pentagram

            AddItem(new Static(5645), 2, 5, 0); // curtain
            AddItem(new Static(5645), 2, 3, 0); // curtain
            AddItem(new Static(5645), 2, 2, 0); // curtain
            AddItem(new Static(5645), 2, 1, 0); // curtain
            AddItem(new Static(5645), 2, 0, 0); // curtain
            AddItem(new Static(5645), 2, -1, 0); // curtain
            AddItem(new Static(5645), 2, -2, 0); // curtain
            AddItem(new Static(5645), 2, -3, 0); // curtain
            AddItem(new Static(5645), 2, -4, 0); // curtain
            AddItem(new Static(5645), 2, -5, 0); // curtain
            AddItem(new Static(5645), 2, -6, 0); // curtain

            AddItem(new Static(5646), 2, -6, 0); // curtain
            AddItem(new Static(5646), 3, -6, 0); // curtain
            AddItem(new Static(5646), 4, -6, 0); // curtain
            AddItem(new Static(5646), 5, -6, 0); // curtain
            AddItem(new Static(5646), 6, -6, 0); // curtain
            AddItem(new Static(5646), 7, -6, 0); // curtain

            AddItem(new Static(7576), 4, -1, 0); // ankh
            AddItem(new Static(7575), 4, 0, 0); // ankh

            AddItem(new Static(6665), -1, -4, 0); // skeleton
            AddItem(new Static(6661), -1, -6, 0); // skeleton
            AddItem(new Static(6658), 3, -6, 0); // skeleton
            AddItem(new Static(6659), 6, -6, 0); // skeleton

            AddItem(new Static(4609), 5, 0, 0); // table
            AddItem(new Static(4611), 5, -1, 0);  // table
            AddItem(new Static(4611), 5, -2, 0);  // table
            AddItem(new Static(4610), 5, -3, 0);  // table

            AddItem(new Static(7400), 5, -2, 0); // torso
            AddItem(new Static(7399), 5, -1, 0); // legs

            AddItem(new Static(7420), 5, -1, 0); // blood
            AddItem(new Static(7418), 5, 0, 0); // blood

            AddMobile(new EvilMage(), 6, 3, 2, 0); // Mage NPC
            AddMobile(new EvilMage(), 6, 3, 2, 0); // Mage NPC
            AddMobile(new EvilMage(), 6, 3, 2, 0); // Mage NPC
            AddMobile(new EvilMage(), 6, 3, 2, 0); // Mage NPC
            AddMobile(new EvilMage(), 6, 2, 3, 0); // Mage NPC

            m_Prisoner = new Noble();
            m_Prisoner.CantWalk = true;
            AddMobile(m_Prisoner, 2, 2, 0, 0); // Prisoner
        }

        public override void OnEnter(Mobile m)
        {
            if (m.Player && m_Prisoner != null && m_Prisoner.CantWalk)
            {
                int number;

                switch (Utility.Random(8))
                {
                    case 0: number = 502261; break; // HELP!
                    case 1: number = 502262; break; // Help me!
                    case 2: number = 502263; break; // Canst thou aid me?!
                    case 3: number = 502264; break; // Help a poor prisoner!
                    case 4: number = 502265; break; // Help! Please!
                    case 5: number = 502266; break; // Aaah! Help me!
                    case 6: number = 502267; break; // Go and get some help!
                    default: number = 502268; break; // Quickly, I beg thee! Unlock my chains! If thou dost look at me close thou canst see them.	
                }
                m_Prisoner.Yell(number);
            }
        }

        public EvilMageCamp(Serial serial) : base(serial)
        {
        }

        public override void AddItem(Item item, int xOffset, int yOffset, int zOffset)
        {
            if (item != null && (item.ItemID == 7418 || item.ItemID == 7420))
                item.Movable = true;

            base.AddItem(item, xOffset, yOffset, zOffset);
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
