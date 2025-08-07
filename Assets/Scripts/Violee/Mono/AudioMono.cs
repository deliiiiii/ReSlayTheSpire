using System.Collections.Generic;
using UnityEngine;

namespace Violee;

public class AudioMono : Singleton<AudioMono>
{
    // ReSharper disable once IdentifierTypo
    public required AudioClip BGMKisekiIns;
    public required List<AudioClip> BGMRecordPlayerIns;
    public required AudioClip BGMVioletLineIns;

    public static AudioClip BGMKiseki => Instance.BGMKisekiIns;
    public static List<AudioClip> BGMRecordPlayer => Instance.BGMRecordPlayerIns;
    public static AudioClip BGMVioletLine => Instance.BGMVioletLineIns;

}