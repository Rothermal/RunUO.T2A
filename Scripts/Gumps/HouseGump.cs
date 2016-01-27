using System;
using System.Collections;
using Server.Network;
using Server.Prompts;
using Server.Multis;
using Server.Items;

namespace Server.Gumps
{
    public class HouseListGump : Gump
    {
        private BaseHouse m_House;

        public HouseListGump(int number, ArrayList list, BaseHouse house, bool accountOf) : base(20, 30)
        {
            if (house.Deleted)
                return;

            m_House = house;

            AddPage(0);

            AddBackground(0, 0, 420, 430, 5054);
            AddBackground(10, 10, 400, 410, 3000);

            AddButton(20, 388, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(55, 388, 300, 20, 1011104, false, false); // Return to previous menu

            AddHtmlLocalized(20, 20, 350, 20, number, false, false);

            if (list != null)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    if ((i % 16) == 0)
                    {
                        if (i != 0)
                        {
                            // Next button
                            AddButton(370, 20, 4005, 4007, 0, GumpButtonType.Page, (i / 16) + 1);
                        }

                        AddPage((i / 16) + 1);

                        if (i != 0)
                        {
                            // Previous button
                            AddButton(340, 20, 4014, 4016, 0, GumpButtonType.Page, i / 16);
                        }
                    }

                    Mobile m = (Mobile)list[i];

                    string name;

                    if (m == null || (name = m.Name) == null || (name = name.Trim()).Length <= 0)
                        continue;

                    AddLabel(55, 55 + ((i % 16) * 20), 0, accountOf && m.Player && m.Account != null ? String.Format("Account of {0}", name) : name);
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (m_House.Deleted)
                return;

            Mobile from = state.Mobile;

            from.SendGump(new HouseGump(from, m_House));
        }
    }

    public class HouseRemoveGump : Gump
    {
        private BaseHouse m_House;
        private ArrayList m_List, m_Copy;
        private int m_Number;
        private bool m_AccountOf;

        public HouseRemoveGump(int number, ArrayList list, BaseHouse house, bool accountOf) : base(20, 30)
        {
            if (house.Deleted)
                return;

            m_House = house;
            m_List = list;
            m_Number = number;
            m_AccountOf = accountOf;

            AddPage(0);

            AddBackground(0, 0, 420, 430, 5054);
            AddBackground(10, 10, 400, 410, 3000);

            AddButton(20, 388, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(55, 388, 300, 20, 1011104, false, false); // Return to previous menu

            AddButton(20, 365, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(55, 365, 300, 20, 1011270, false, false); // Remove now!

            AddHtmlLocalized(20, 20, 350, 20, number, false, false);

            if (list != null)
            {
                m_Copy = new ArrayList(list);

                for (int i = 0; i < list.Count; ++i)
                {
                    if ((i % 15) == 0)
                    {
                        if (i != 0)
                        {
                            // Next button
                            AddButton(370, 20, 4005, 4007, 0, GumpButtonType.Page, (i / 15) + 1);
                        }

                        AddPage((i / 15) + 1);

                        if (i != 0)
                        {
                            // Previous button
                            AddButton(340, 20, 4014, 4016, 0, GumpButtonType.Page, i / 15);
                        }
                    }

                    Mobile m = (Mobile)list[i];

                    string name;

                    if (m == null || (name = m.Name) == null || (name = name.Trim()).Length <= 0)
                        continue;

                    AddCheck(34, 52 + ((i % 15) * 20), 0xD2, 0xD3, false, i);
                    AddLabel(55, 52 + ((i % 15) * 20), 0, accountOf && m.Player && m.Account != null ? String.Format("Account of {0}", name) : name);
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (m_House.Deleted)
                return;

            Mobile from = state.Mobile;

            if (m_List != null && info.ButtonID == 1) // Remove now
            {
                if (m_House.IsOwner( from ))
                {
                    if (m_House.InternalizedVendors.Count > 0)
                    {
                        return;
                    }
                    else if (!Guilds.Guild.NewGuildSystem && m_House.FindGuildstone() != null)
                    {
                        from.SendLocalizedMessage(501389); // You cannot redeed a house with a guildstone inside.
                        return;
                    }
                    else if (m_House.PlayerVendors.Count > 0)
                    {
                        from.SendLocalizedMessage(503236); // You need to collect your vendor's belongings before moving.
                        return;
                    }
                    else if (m_House.VendorInventories.Count > 0)
                    {
                        from.SendLocalizedMessage(1062681); // You cannot do that that while you still have unclaimed contract vendor inventory in your house.
                        return;
                    }

                    Item toGive = m_House.GetDeed();

                    if (toGive != null)
                    {
                        Container backpack = from.Backpack;

                        if (backpack == null)
                        {
                            from.SendLocalizedMessage(501393); // You do not seem to have a backpack.
                        }
                        else if (backpack.TryDropItem(from, toGive, false))
                        {
                            from.SendLocalizedMessage(501391); // You place the deed in your backpack.
                            m_House.RemoveKeys(from);
                            m_House.Delete();
                        }
                        else
                        {
                            from.SendLocalizedMessage(501390); // You do not have room in your backpack for a house deed.
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(501294); // Redeeding the house has failed.
                    }
                }
                else
                {
                    from.SendLocalizedMessage(501320); // Only the house owner may do this.
                }
            }

            int[] switches = info.Switches;

            if (switches.Length > 0)
            {
                for (int i = 0; i < switches.Length; ++i)
                {
                    int index = switches[i];

                    if (index >= 0 && index < m_Copy.Count)
                        m_List.Remove(m_Copy[index]);
                }

                if (m_List.Count > 0)
                {
                    from.CloseGump(typeof(HouseGump));
                    from.CloseGump(typeof(HouseListGump));
                    from.CloseGump(typeof(HouseRemoveGump));
                    from.SendGump(new HouseRemoveGump(m_Number, m_List, m_House, m_AccountOf));
                    return;
                }
            }

            from.SendGump(new HouseGump(from, m_House));
        }
}

public class HouseGump : Gump
{
    private BaseHouse m_House;

    private ArrayList Wrap(string value)
    {
        if (value == null || (value = value.Trim()).Length <= 0)
            return null;

        string[] values = value.Split(' ');
        ArrayList list = new ArrayList();
        string current = "";

        for (int i = 0; i < values.Length; ++i)
        {
            string val = values[i];

            string v = current.Length == 0 ? val : current + ' ' + val;

            if (v.Length < 10)
            {
                current = v;
            }
            else if (v.Length == 10)
            {
                list.Add(v);

                if (list.Count == 6)
                    return list;

                current = "";
            }
            else if (val.Length <= 10)
            {
                list.Add(current);

                if (list.Count == 6)
                    return list;

                current = val;
            }
            else
            {
                while (v.Length >= 10)
                {
                    list.Add(v.Substring(0, 10));

                    if (list.Count == 6)
                        return list;

                    v = v.Substring(10);
                }

                current = v;
            }
        }

        if (current.Length > 0)
            list.Add(current);

        return list;
    }

    public HouseGump(Mobile from, BaseHouse house) : base(20, 30)
    {
        if (house.Deleted)
            return;

        m_House = house;

        from.CloseGump(typeof(HouseGump));
        from.CloseGump(typeof(HouseListGump));
        from.CloseGump(typeof(HouseRemoveGump));

        bool isCombatRestricted = house.IsCombatRestricted(from);

        bool isOwner = m_House.IsOwner(from);
        bool isCoOwner = isOwner || m_House.IsCoOwner(from);
        bool isFriend = isCoOwner || m_House.IsFriend(from);

        if (isCombatRestricted)
            isFriend = isCoOwner = isOwner = false;

        AddPage(0);

        if (isFriend)
        {
            AddBackground(0, 40, 380, 285, 0xA28);
            AddBackground(15, 100, 350, 25, 0x13EC);
        }

        AddImage(120, 0, 100);

        if (m_House.Sign != null)
        {
            ArrayList lines = Wrap(m_House.Sign.GetName());

            if (lines != null)
            {
                for (int i = 0, y = (101 - (lines.Count * 14)) / 2; i < lines.Count; ++i, y += 14)
                {
                    string s = (string)lines[i];
                    AddLabel(130 + ((143 - (s.Length * 8)) / 2), y, 0x455, (string)lines[i]);
                }
            }
        }

        if (!isFriend)
            return;

        AddLabel(30, 102, 0x0, "INFO");
        AddButton(80, 104, 0x1459, 0x1459, 0, GumpButtonType.Page, 1);

        AddLabel(120, 102, 0x0, "FRIENDS");
        AddButton(210, 104, 0x1459, 0x1459, 0, GumpButtonType.Page, 2);

        AddLabel(245, 102, 0x0, "OPTIONS");
        AddButton(335, 103, 0x1459, 0x1459, 0, GumpButtonType.Page, 3);

        AddButton(50, 290, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);

        // Info page
        AddPage(1);

        AddLabel(100, 140, 0x0, "Owned by");
        AddLabel(180, 140, 0x455, GetOwnerName());

        AddLabel(30, 170, 0x0, "Number of locked down items:");
        AddLabel(250, 170, 0x0, m_House.LockDownCount.ToString() + " out of " + m_House.MaxLockDowns.ToString());

        AddLabel(30, 190, 0x0, "Number of secure containers:");
        AddLabel(250, 190, 0x0, m_House.SecureCount.ToString() + " out of " + m_House.MaxSecures.ToString());

        AddLabel(30, 240, 0x0, "This house is of modern design");

        if (m_House.Public)
        {
            AddHtmlLocalized(30, 260, 275, 20, 1011241, false, false);
            AddHtml(320, 260, 50, 20, m_House.Visits.ToString(), false, false);
        }

        AddLabel(130, 290, 0x0, "Change the house's name!");
        AddButton(310, 290, 0xA9A, 0xA9B, 1, GumpButtonType.Reply, 0);

        // Friends page
        AddPage(2);

        AddLabel(55, 127, 0x0, "List of Co-Owners");
        AddButton(30, 127, 0xA9A, 0xA9B, 2, GumpButtonType.Reply, 0);

        AddLabel(55, 147, 0x0, "Add a Co-Owner");
        AddButton(30, 147, 0xA9A, 0xA9B, 3, GumpButtonType.Reply, 0);

        AddLabel(55, 167, 0x0, "Remove a Co-Owner");
        AddButton(30, 167, 0xA9A, 0xA9B, 4, GumpButtonType.Reply, 0);

        AddLabel(200, 127, 0x0, "List of Friends");
        AddButton(175, 127, 0xA9A, 0xA9B, 6, GumpButtonType.Reply, 0);

        AddLabel(200, 147, 0x0, "Add a Friend");
        AddButton(175, 147, 0xA9A, 0xA9B, 7, GumpButtonType.Reply, 0);

        AddLabel(200, 167, 0x0, "Remove a Friend");
        AddButton(175, 167, 0xA9A, 0xA9B, 8, GumpButtonType.Reply, 0);

        AddLabel(200, 187, 0x0, "Clear Friend List");
        AddButton(175, 187, 0xA9A, 0xA9B, 9, GumpButtonType.Reply, 0);

        AddLabel(100, 207, 0x0, "Ban someone from the house");
        AddButton(75, 207, 0xA9A, 0xA9B, 10, GumpButtonType.Reply, 0);

        AddLabel(100, 227, 0x0, "Eject someone from the house");
        AddButton(75, 227, 0xA9A, 0xA9B, 11, GumpButtonType.Reply, 0);

        AddLabel(100, 247, 0x0, "View list of banned people");
        AddButton(75, 247, 0xA9A, 0xA9B, 12, GumpButtonType.Reply, 0);

        AddLabel(100, 267, 0x0, "Lift a Ban");
        AddButton(75, 267, 0xA9A, 0xA9B, 13, GumpButtonType.Reply, 0);

        // Options page
        AddPage(3);

        AddLabel(55, 140, 0x0, "Transfer ownership of the house");
        AddButton(30, 140, 0xA9A, 0xA9B, 14, GumpButtonType.Reply, 0);

        AddLabel(55, 170, 0x0, "Demolish the house and get a deed back");
        AddButton(30, 170, 0xA9A, 0xA9B, 15, GumpButtonType.Reply, 0);

        if (!m_House.Public)
        {
            AddLabel(55, 200, 0x0, "Change the house locks");
            AddButton(30, 200, 0xA9A, 0xA9B, 16, GumpButtonType.Reply, 0);

            AddLabel(55, 230, 0x0, "Declare this building to be public. This will make your front door unlockable.");
            AddButton(30, 230, 0xA9A, 0xA9B, 17, GumpButtonType.Reply, 0);
        }
        else
        {
            AddLabel(55, 200, 0x0, "Change the sign type");
            AddButton(30, 200, 2714, 2715, 0, GumpButtonType.Page, 4);

            AddLabel(55, 230, 0x0, "Declare this building to be private.");
            AddButton(30, 230, 2714, 2715, 17, GumpButtonType.Reply, 0);

            // Change the sign type
            AddPage(4);

            for (int i = 0; i < 24; ++i)
            {
                AddRadio(53 + ((i / 4) * 50), 137 + ((i % 4) * 35), 210, 211, false, i + 1);
                AddItem(60 + ((i / 4) * 50), 130 + ((i % 4) * 35), 2980 + (i * 2));
            }

            AddLabel(200, 305, 0x0, "Guild sign choices");
            AddButton(350, 305, 252, 253, 0, GumpButtonType.Page, 5);

            AddLabel(200, 340, 0x0, "Okay that is fine.");
            AddButton(350, 340, 4005, 4007, 18, GumpButtonType.Reply, 0);

            AddPage(5);

            for (int i = 0; i < 29; ++i)
            {
                AddRadio(53 + ((i / 5) * 50), 137 + ((i % 5) * 35), 210, 211, false, i + 25);
                AddItem(60 + ((i / 5) * 50), 130 + ((i % 5) * 35), 3028 + (i * 2));
            }

            AddLabel(200, 260, 0x0, "Shop sign choices");
            AddButton(350, 260, 250, 251, 0, GumpButtonType.Page, 4);

            AddLabel(200, 290, 0x0, "Okay that is fine.");
            AddButton(350, 290, 4005, 4007, 18, GumpButtonType.Reply, 0);
        }
        AddPage(6); // Demolish House Menu ??
    }

    private string GetOwnerName()
    {
        Mobile m = m_House.Owner;

        if (m == null)
            return "(unowned)";

        string name;

        if ((name = m.Name) == null || (name = name.Trim()).Length <= 0)
            name = "(no name)";

        return name;
    }

    public override void OnResponse(NetState sender, RelayInfo info)
    {
        if (m_House.Deleted)
            return;

        Mobile from = sender.Mobile;

        bool isCombatRestricted = m_House.IsCombatRestricted(from);

        bool isOwner = m_House.IsOwner(from);
        bool isCoOwner = isOwner || m_House.IsCoOwner(from);
        bool isFriend = isCoOwner || m_House.IsFriend(from);

        if (isCombatRestricted)
            isFriend = isCoOwner = isOwner = false;

        if (!isFriend || !from.Alive)
            return;

        Item sign = m_House.Sign;

        if (sign == null || from.Map != sign.Map || !from.InRange(sign.GetWorldLocation(), 18))
            return;

        switch (info.ButtonID)
        {
            case 1: // Rename sign
                {
                    from.Prompt = new RenamePrompt(m_House);
                    from.SendLocalizedMessage(501302); // What dost thou wish the sign to say?

                    break;
                }
            case 2: // List of co-owners
                {
                    from.CloseGump(typeof(HouseGump));
                    from.CloseGump(typeof(HouseListGump));
                    from.CloseGump(typeof(HouseRemoveGump));
                    from.SendGump(new HouseListGump(1011275, m_House.CoOwners, m_House, false));

                    break;
                }
            case 3: // Add co-owner
                {
                    if (isOwner)
                    {
                        from.SendLocalizedMessage(501328); // Target the person you wish to name a co-owner of your household.
                        from.Target = new CoOwnerTarget(true, m_House);
                    }
                    else
                    {
                        from.SendLocalizedMessage(501327); // Only the house owner may add Co-owners.
                    }

                    break;
                }
            case 4: // Remove co-owner
                {
                    if (isOwner)
                    {
                        from.CloseGump(typeof(HouseGump));
                        from.CloseGump(typeof(HouseListGump));
                        from.CloseGump(typeof(HouseRemoveGump));
                        from.SendGump(new HouseRemoveGump(1011274, m_House.CoOwners, m_House, false));
                    }
                    else
                    {
                        from.SendLocalizedMessage(501329); // Only the house owner may remove co-owners.
                    }

                    break;
                }
            case 5: // Clear co-owners
                {
                    if (isOwner)
                    {
                        if (m_House.CoOwners != null)
                            m_House.CoOwners.Clear();

                        from.SendLocalizedMessage(501333); // All co-owners have been removed from this house.
                    }
                    else
                    {
                        from.SendLocalizedMessage(501330); // Only the house owner may remove co-owners.
                    }

                    break;
                }
            case 6: // List friends
                {
                    from.CloseGump(typeof(HouseGump));
                    from.CloseGump(typeof(HouseListGump));
                    from.CloseGump(typeof(HouseRemoveGump));
                    from.SendGump(new HouseListGump(1011273, m_House.Friends, m_House, false));

                    break;
                }
            case 7: // Add friend
                {
                    if (isCoOwner)
                    {
                        from.SendLocalizedMessage(501317); // Target the person you wish to name a friend of your household.
                        from.Target = new HouseFriendTarget(true, m_House);
                    }
                    else
                    {
                        from.SendLocalizedMessage(501316); // Only the house owner may add friends.
                    }

                    break;
                }
            case 8: // Remove friend
                {
                    if (isCoOwner)
                    {
                        from.CloseGump(typeof(HouseGump));
                        from.CloseGump(typeof(HouseListGump));
                        from.CloseGump(typeof(HouseRemoveGump));
                        from.SendGump(new HouseRemoveGump(1011272, m_House.Friends, m_House, false));
                    }
                    else
                    {
                        from.SendLocalizedMessage(501318); // Only the house owner may remove friends.
                    }

                    break;
                }
            case 9: // Clear friends
                {
                    if (isCoOwner)
                    {
                        if (m_House.Friends != null)
                            m_House.Friends.Clear();

                        from.SendLocalizedMessage(501332); // All friends have been removed from this house.
                    }
                    else
                    {
                        from.SendLocalizedMessage(501319); // Only the house owner may remove friends.
                    }

                    break;
                }
            case 10: // Ban
                {
                    from.SendLocalizedMessage(501325); // Target the individual to ban from this house.
                    from.Target = new HouseBanTarget(true, m_House);

                    break;
                }
            case 11: // Eject
                {
                    from.SendLocalizedMessage(501326); // Target the individual to eject from this house.
                    from.Target = new HouseKickTarget(m_House);

                    break;
                }
            case 12: // List bans
                {
                    from.CloseGump(typeof(HouseGump));
                    from.CloseGump(typeof(HouseListGump));
                    from.CloseGump(typeof(HouseRemoveGump));
                    from.SendGump(new HouseListGump(1011271, m_House.Bans, m_House, true));

                    break;
                }
            case 13: // Remove ban
                {
                    from.CloseGump(typeof(HouseGump));
                    from.CloseGump(typeof(HouseListGump));
                    from.CloseGump(typeof(HouseRemoveGump));
                    from.SendGump(new HouseRemoveGump(1011269, m_House.Bans, m_House, true));

                    break;
                }
            case 14: // Transfer ownership
                {
                    if (isOwner)
                    {
                        from.SendLocalizedMessage(501309); // Target the person to whom you wish to give this house.
                        from.Target = new HouseOwnerTarget(m_House);
                    }
                    else
                    {
                        from.SendLocalizedMessage(501310); // Only the house owner may do this.
                    }

                    break;
                }
            case 15: // Demolish house
                {
                    if (isOwner)
                    {
                        if (!Guilds.Guild.NewGuildSystem && m_House.FindGuildstone() != null)
                        {
                            from.SendLocalizedMessage(501389); // You cannot redeed a house with a guildstone inside.
                        }
                        else
                        {

                            from.CloseGump(typeof(HouseGump));
                            from.CloseGump(typeof(HouseListGump));
                            from.CloseGump(typeof(HouseRemoveGump));

                            from.SendGump(new HouseRemoveGump(1011249, m_House.Bans, m_House, true));
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(501320); // Only the house owner may do this.
                    }

                    break;
                }
            case 16: // Change locks
                {
                    if (m_House.Public)
                    {
                        from.SendLocalizedMessage(501669);// Public houses are always unlocked.
                    }
                    else
                    {
                        if (isOwner)
                        {
                            m_House.RemoveKeys(from);
                            m_House.ChangeLocks(from);

                            from.SendLocalizedMessage(501306); // The locks on your front door have been changed, and new master keys have been placed in your bank and your backpack.
                        }
                        else
                        {
                            from.SendLocalizedMessage(501303); // Only the house owner may change the house locks.
                        }
                    }

                    break;
                }
            case 17: // Declare public/private
                {
                    if (isOwner)
                    {
                        if (m_House.Public && m_House.PlayerVendors.Count > 0)
                        {
                            from.SendLocalizedMessage(501887); // You have vendors working out of this building. It cannot be declared private until there are no vendors in place.
                            break;
                        }

                        m_House.Public = !m_House.Public;
                        if (!m_House.Public)
                        {
                            m_House.ChangeLocks(from);

                            from.SendLocalizedMessage(501888); // This house is now private.
                            from.SendLocalizedMessage(501306); // The locks on your front door have been changed, and new master keys have been placed in your bank and your backpack.
                        }
                        else
                        {
                            m_House.RemoveKeys(from);
                            m_House.RemoveLocks();
                            from.SendLocalizedMessage(501886);//This house is now public. Friends of the house my now have vendors working out of this building.
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(501307); // Only the house owner may do this.
                    }

                    break;
                }
            case 18: // Change type
                {
                    if (isOwner)
                    {
                        if (m_House.Public && info.Switches.Length > 0)
                        {
                            int index = info.Switches[0] - 1;

                            if (index >= 0 && index < 53)
                                m_House.ChangeSignType(2980 + (index * 2));
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(501307); // Only the house owner may do this.
                    }

                    break;
                }
        }
    }
}
}

namespace Server.Prompts
{
    public class RenamePrompt : Prompt
    {
        private BaseHouse m_House;

        public RenamePrompt(BaseHouse house)
        {
            m_House = house;
        }

        public override void OnResponse(Mobile from, string text)
        {
            if (m_House.IsFriend(from))
            {
                if (m_House.Sign != null)
                    m_House.Sign.Name = text;

                from.SendMessage("Sign changed.");
            }
        }
    }
}