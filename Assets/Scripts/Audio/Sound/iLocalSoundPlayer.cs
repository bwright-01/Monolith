
using Core;

namespace Audio {
    namespace Sound {

        interface iLocalSoundPlayer {
            event StringEvent OnPlaySound;
            void PlaySound(string soundName);
        }
    }
}
