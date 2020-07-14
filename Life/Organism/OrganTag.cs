namespace Life
{
    public class OrganTag
    {
    }

    #region Дыхание

    public class RespiratoryOrgan : OrganTag
    {
    }

    public class Lung : RespiratoryOrgan
    {
    }

    public class Gills : RespiratoryOrgan
    {
    }

    #endregion

    #region Восприятие

    public class PerceptionOrgan : OrganTag
    {
    }

    public class Eye : PerceptionOrgan
    {
    }

    public class Ear : PerceptionOrgan
    {
    }

    public class ThermaVision : PerceptionOrgan
    {
    }

    public class BloodDetector : PerceptionOrgan
    {
    }

    public class Nose : PerceptionOrgan
    {
    }

    public class StereoNose : Nose
    {
    }

    public class Antenna : PerceptionOrgan
    {
    }

    #endregion

    #region Пищеварение

    public class DigestionOrgan : OrganTag
    {
    }

    public class Stomach : DigestionOrgan
    {
    }

    #endregion

    #region Конечности

    public class Limb : OrganTag
    {
    }

    public class Hand : Limb
    {
    }

    public class Wing : Limb
    {
    }

    public class Leg: Limb
    {
    }

    public class Paw : Limb
    {
    }

    public class Tentacle : Limb
    {
    }

    public class Jumper : Limb
    {
    }

    public class Chela : Limb
    {
    }

    public class Flashlight : Limb
    {
    }
    #endregion

    #region Покров

    public class Cover : OrganTag
    {
    }

    #endregion

    #region ТипаКровь

    public class QuasiBlood : OrganTag
    {
    }

    public class Blood : QuasiBlood
    {
    }

    public class TreeJuice : QuasiBlood
    {
    }

    public class Hemolymph : QuasiBlood
    {
    }

    #endregion

    #region Перенос типа крови

    public class QuasiHeart : OrganTag
    {
    }

    public class Heart : QuasiHeart
    {
    }

    #endregion
}