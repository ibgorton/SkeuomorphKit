using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalNumericUpdown
{
    /* Segment Numbering
    *         _____________   _____________
    *        /     ONE     | |     TWO     \ 
    *      __\_____________| |_____________/__
    *     /   \/ \        |---|        / \/   \
    *     |   |   \       |   |       /   |   |
    *     |   |\   \      |   |      /   /|   |
    *     | E | \ N \     | T |     / N / | T |
    *     | I |  \ I \    | E |    / E /  | H |
    *     | G |   \ N \   | N |   / V /   | R |
    *     | H |    \ E \  |   |  / E /    | E |
    *     | T |     \   \ |   | / L /     | E |
    *     |   |      \   \|   |/ E /      |   |
    *     |   | ______\___|---|___/______ |   |
    *     \___// SIXTEEN   | |   TWELVE  \\___/
    *     /   \\___________| |___________//   \
    *     |   |       /   |---|   \       |   |
    *     |   |      / N /| F |\ T \      |   |
    *     | S |     / E / | O | \ H \     | F |
    *     | E |    / E /  | U |  \ I \    | O |
    *     | V |   / T /   | R |   \ R \   | U |
    *     | E |  / F /    | T |    \ T \  | R | 
    *     | N | / I /     | E |     \ E \ |   |
    *     |   |/ F /      | E |      \ E \|   |
    *     |   |   /       | N |       \ N |   |
    *     \___/\_/________|---|________\_/\___/  
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

        public override void Increment()
        {
            throw new NotImplementedException();
        }

        public override void Decrement()
        {
            throw new NotImplementedException();
        }
    }
}
