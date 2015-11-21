using System;
using System.Collections;
using Server.Commands;

namespace Server.Items
{
    public class StealableArtifactsSpawner : Item
	{
		public class StealableEntry
		{
			private Map m_Map;
			private Point3D m_Location;
			private int m_MinDelay;
			private int m_MaxDelay;
			private Type m_Type;
			private int m_Hue;

			public Map Map{ get{ return m_Map; } }
			public Point3D Location{ get{ return m_Location; } }
			public int MinDelay{ get{ return m_MinDelay; } }
			public int MaxDelay{ get{ return m_MaxDelay; } }
			public Type Type{ get{ return m_Type; } }
			public int Hue{ get{ return m_Hue; } }

			public StealableEntry( Map map, Point3D location, int minDelay, int maxDelay, Type type ) : this( map, location, minDelay, maxDelay, type, 0 )
			{
			}

			public StealableEntry( Map map, Point3D location, int minDelay, int maxDelay, Type type, int hue )
			{
				m_Map = map;
				m_Location = location;
				m_MinDelay = minDelay;
				m_MaxDelay = maxDelay;
				m_Type = type;
				m_Hue = hue;
			}

			public Item CreateInstance()
			{
				Item item = (Item) Activator.CreateInstance( m_Type );

				if ( m_Hue > 0 )
					item.Hue = m_Hue;

				item.Movable = false;
				item.MoveToWorld( this.Location, this.Map );

				return item;
			}
		}

		private static StealableEntry[] m_Entries = new StealableEntry[]
			{
			};

		public static StealableEntry[] Entries{ get{ return m_Entries; } }

		private static Type[] m_TypesOfEntries = null;
		public static Type[] TypesOfEntires
		{
			get
			{
				if( m_TypesOfEntries == null )
				{
					m_TypesOfEntries = new Type[m_Entries.Length];

					for( int i = 0; i < m_Entries.Length; i++ )
						m_TypesOfEntries[i] = m_Entries[i].Type;
				}

				return m_TypesOfEntries;
			}
		}

		private static StealableArtifactsSpawner m_Instance;

		public static StealableArtifactsSpawner Instance{ get{ return m_Instance; } }

		private static int GetLampPostHue()
		{
			if ( 0.9 > Utility.RandomDouble() )
				return 0;

			return Utility.RandomList( 0x455, 0x47E, 0x482, 0x486, 0x48F, 0x4F2, 0x58C, 0x66C );
		}


		public static void Initialize()
		{
			CommandSystem.Register( "GenStealArties", AccessLevel.Administrator, new CommandEventHandler( GenStealArties_OnCommand ) );
			CommandSystem.Register( "RemoveStealArties", AccessLevel.Administrator, new CommandEventHandler( RemoveStealArties_OnCommand ) );
		}

		[Usage( "GenStealArties" )]
		[Description( "Generates the stealable artifacts spawner." )]
		private static void GenStealArties_OnCommand( CommandEventArgs args )
		{
			Mobile from = args.Mobile;

			if ( Create() )
				from.SendMessage( "Stealable artifacts spawner generated." );
			else
				from.SendMessage( "Stealable artifacts spawner already present." );
		}

		[Usage( "RemoveStealArties" )]
		[Description( "Removes the stealable artifacts spawner and every not yet stolen stealable artifacts." )]
		private static void RemoveStealArties_OnCommand( CommandEventArgs args )
		{
			Mobile from = args.Mobile;

			if ( Remove() )
				from.SendMessage( "Stealable artifacts spawner removed." );
			else
				from.SendMessage( "Stealable artifacts spawner not present." );
		}

		public static bool Create()
		{
			if ( m_Instance != null && !m_Instance.Deleted )
				return false;

			m_Instance = new StealableArtifactsSpawner();
			return true;
		}

		public static bool Remove()
		{
			if ( m_Instance == null )
				return false;

			m_Instance.Delete();
			m_Instance = null;
			return true;
		}

		public static StealableInstance GetStealableInstance( Item item )
		{
			if ( Instance == null )
				return null;

			return (StealableInstance) Instance.m_Table[item];
		}


		public class StealableInstance
		{
			private StealableEntry m_Entry;
			private Item m_Item;
			private DateTime m_NextRespawn;

			public StealableEntry Entry{ get{ return m_Entry; } }

			public Item Item
			{
				get{ return m_Item; }
				set
				{
					if ( m_Item != null && value == null )
					{
						int delay = Utility.RandomMinMax( this.Entry.MinDelay, this.Entry.MaxDelay );
						this.NextRespawn = DateTime.Now + TimeSpan.FromMinutes( delay );
					}

					if ( Instance != null )
					{
						if ( m_Item != null	)
							Instance.m_Table.Remove( m_Item );

						if ( value != null )
							Instance.m_Table[value] = this;
					}

					m_Item = value;
				}
			}

			public DateTime NextRespawn
			{
				get{ return m_NextRespawn; }
				set{ m_NextRespawn = value; }
			}

			public StealableInstance( StealableEntry entry ) : this( entry, null, DateTime.Now )
			{
			}

			public StealableInstance( StealableEntry entry, Item item, DateTime nextRespawn )
			{
				m_Item = item;
				m_NextRespawn = nextRespawn;
				m_Entry = entry;
			}

			public void CheckRespawn()
			{
				if ( this.Item != null && ( this.Item.Deleted || this.Item.Movable || this.Item.Parent != null ) )
					this.Item = null;

				if ( this.Item == null && DateTime.Now >= this.NextRespawn )
				{
					this.Item = this.Entry.CreateInstance();
				}
			}
		}

		private Timer m_RespawnTimer;
		private StealableInstance[] m_Artifacts;
		private Hashtable m_Table;

		public override string DefaultName
		{
			get { return "Stealable Artifacts Spawner - Internal"; }
		}

		private StealableArtifactsSpawner() : base( 1 )
		{
			Movable = false;

			m_Artifacts = new StealableInstance[m_Entries.Length];
			m_Table = new Hashtable( m_Entries.Length );

			for ( int i = 0; i < m_Entries.Length; i++ )
			{
				m_Artifacts[i] = new StealableInstance( m_Entries[i] );
			}

			m_RespawnTimer = Timer.DelayCall( TimeSpan.Zero, TimeSpan.FromMinutes( 15.0 ), new TimerCallback( CheckRespawn ) );
		}

		public override void OnDelete()
		{
			base.OnDelete();

			if ( m_RespawnTimer != null )
			{
				m_RespawnTimer.Stop();
				m_RespawnTimer = null;
			}

			foreach ( StealableInstance si in m_Artifacts )
			{
				if ( si.Item != null )
					si.Item.Delete();
			}

			m_Instance = null;
		}

		public void CheckRespawn()
		{
			foreach ( StealableInstance si in m_Artifacts )
			{
				si.CheckRespawn();
			}
		}

		public StealableArtifactsSpawner( Serial serial ) : base( serial )
		{
			m_Instance = this;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version

			writer.WriteEncodedInt( m_Artifacts.Length );

			for ( int i = 0; i < m_Artifacts.Length; i++ )
			{
				StealableInstance si = m_Artifacts[i];

				writer.Write( (Item) si.Item );
				writer.WriteDeltaTime( (DateTime) si.NextRespawn );
			}
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_Artifacts = new StealableInstance[m_Entries.Length];
			m_Table = new Hashtable( m_Entries.Length );

			int length = reader.ReadEncodedInt();

			for ( int i = 0; i < length; i++ )
			{
				Item item = reader.ReadItem();
				DateTime nextRespawn = reader.ReadDeltaTime();

				if ( i < m_Artifacts.Length )
				{
					StealableInstance si = new StealableInstance( m_Entries[i], item, nextRespawn );
					m_Artifacts[i] = si;

					if ( si.Item != null )
						m_Table[si.Item] = si;
				}
			}

			for ( int i = length; i < m_Entries.Length; i++ )
			{
				m_Artifacts[i] = new StealableInstance( m_Entries[i] );
			}

			m_RespawnTimer = Timer.DelayCall( TimeSpan.Zero, TimeSpan.FromMinutes( 15.0 ), new TimerCallback( CheckRespawn ) );
		}
	}
}