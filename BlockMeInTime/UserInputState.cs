using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockMeInTime
{
    enum UserInputStateEnum
    {
        DEFAULT,
        DRAGGING_SELECTION,
        SELECTED_EVENT
    }

    class UserInputState
    {
        public UserInputStateEnum state;

        private static UserInputState user_input_state = null;

        private UserInputState()
        {
            state = UserInputStateEnum.DEFAULT;
        }

        public static UserInputState GetUserInputState()
        {
            if (user_input_state == null)
                user_input_state = new UserInputState();

            return user_input_state;
        }
    }
}
