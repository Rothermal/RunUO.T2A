using Server.Network;

namespace Server.Targeting
{
    public abstract class MultiTarget : Target
	{
		private int m_MultiID;
		private Point3D m_Offset;

		public int MultiID {  get { return m_MultiID; } set { m_MultiID = value; } }
    	public Point3D Offset { get { return m_Offset; } set { m_Offset = value; } }

		protected MultiTarget( int multiID, Point3D offset ) : this( multiID, offset, 10, true, TargetFlags.None )
		{
		}

		protected MultiTarget( int multiID, Point3D offset, int range, bool allowGround, TargetFlags flags ) : base( range, allowGround, flags )
		{
			m_MultiID = multiID;
			m_Offset = offset;
		}

		public override Packet GetPacketFor( NetState ns )
		{
			if ( ns.HighSeas )
				return new MultiTargetReqHS( this );
			else
				return new MultiTargetReq( this );
		}
	}
}