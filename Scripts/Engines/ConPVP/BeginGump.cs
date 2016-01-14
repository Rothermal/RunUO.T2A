using System;
using Server.Gumps;

namespace Server.Engines.ConPVP
{
    public class BeginGump : Gump
	{
		public string Center( string text )
		{
			return String.Format( "<CENTER>{0}</CENTER>", text );
		}

		public string Color( string text, int color )
		{
			return String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text );
		}

		private const int LabelColor32 = 0xFFFFFF;
		private const int BlackColor32 = 0x000008;

		public BeginGump() : base( 50, 50 )
		{
			AddPage( 0 );

			const int offset = 50;

			AddBackground( 1, 1, 398, 202-offset, 3600 );

			AddImageTiled( 16, 15, 369, 173-offset, 3604 );
			AddAlphaRegion( 16, 15, 369, 173-offset );

			AddImage( 215, -43, 0xEE40 );

			AddHtml( 22-1, 22, 294, 20, Color( Center( "Duel Countdown" ), BlackColor32 ), false, false );
			AddHtml( 22+1, 22, 294, 20, Color( Center( "Duel Countdown" ), BlackColor32 ), false, false );
			AddHtml( 22, 22-1, 294, 20, Color( Center( "Duel Countdown" ), BlackColor32 ), false, false );
			AddHtml( 22, 22+1, 294, 20, Color( Center( "Duel Countdown" ), BlackColor32 ), false, false );
			AddHtml( 22, 22, 294, 20, Color( Center( "Duel Countdown" ), LabelColor32 ), false, false );

			AddHtml( 22-1, 50, 294, 80, Color( "The arranged duel is about to begin. During this countdown period you may not cast spells and you may not move. This message will close automatically when the period ends.", BlackColor32 ), false, false );
			AddHtml( 22+1, 50, 294, 80, Color( "The arranged duel is about to begin. During this countdown period you may not cast spells and you may not move. This message will close automatically when the period ends.", BlackColor32 ), false, false );
			AddHtml( 22, 50-1, 294, 80, Color( "The arranged duel is about to begin. During this countdown period you may not cast spells and you may not move. This message will close automatically when the period ends.", BlackColor32 ), false, false );
			AddHtml( 22, 50+1, 294, 80, Color( "The arranged duel is about to begin. During this countdown period you may not cast spells and you may not move. This message will close automatically when the period ends.", BlackColor32 ), false, false );
			AddHtml( 22, 50, 294, 80, Color( "The arranged duel is about to begin. During this countdown period you may not cast spells and you may not move. This message will close automatically when the period ends.", 0xFFCC66 ), false, false );

			AddButton( 314-50, 157-offset, 247, 248, 1, GumpButtonType.Reply, 0 );
		}
	}
}