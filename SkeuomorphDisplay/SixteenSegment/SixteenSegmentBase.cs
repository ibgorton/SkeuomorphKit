using System;

namespace SkeuomorphDisplay.SixteenSegment
{
    /*              SEGMENT NUMBERING
    *         _____________   _____________
    *        /     ONE     | |     TWO     \ 
    *      __\_____________|_|_____________/__
    *     /   \  \        |   |        /  /   \
    *     |   |   \       |   |       /   |   |
    *     |   |\   \      |   |      /   /|   |
    *     | E | \ N \     | T |     / N / | T |
    *     | I |  \ I \    | E |    / E /  | H |
    *     | G |   \ N \   | N |   / V /   | R |
    *     | H |    \ E \  |   |  / E /    | E |
    *     | T |     \   \ |   | / L /     | E |
    *     |   |      \   \|   |/ E /      |   |
    *     |   |_______\___|___|___/_______|   |
    *     \___/  SIXTEEN   | |   TWELVE   \___/
    *     /   \____________|_|____________/   \
    *     |   |       /   |   |   \       |   |
    *     |   |      / N /| F |\ T \      |   |
    *     | S |     / E / | O | \ H \     | F |
    *     | E |    / E /  | U |  \ I \    | O |
    *     | V |   / T /   | R |   \ R \   | U |
    *     | E |  / F /    | T |    \ T \  | R | 
    *     | N | / I /     | E |     \ E \ |   |
    *     |   |/ F /      | E |      \ E \|   |
    *     |   |   /       | N |       \ N |   |
    *     \___/__/________|___|________\__\___/  
    *        /       SIX   | |  FIVE       \     /--\  <- DECIMAL POINT
    *        \_____________| |_____________/     \--/
    */

    public abstract class SixteenSegmentBase : DisplayControlBase
    {
        protected SixteenSegmentBase()
        {
        }

        public override void BlankModule()
        {
            throw new NotImplementedException();
        }

        public override void SetChar(char character)
        {
            throw new NotImplementedException();
        }

    }
}
