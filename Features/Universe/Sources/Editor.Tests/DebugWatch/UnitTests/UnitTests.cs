using Universe.DebugWatch.Runtime;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

public class UnitTests
{
    #region Validate the ValidateMethods

    [Test]
    public static void ValidateTheValidation()
    {
        DebugMenuRegistry.InitializeMethods();
    }

    #endregion


    #region Valid me that path

    [Test]
    public static void ValidAPath()
    {
        var path                = "Enter your static path to test here";
        var myPathsFromGetPaths = DebugMenuRegistry.Paths;
        var myPathsToTest       = new List<string>();
        var result              = false;

        myPathsToTest.AddRange(myPathsFromGetPaths.ToList());

        result = myPathsToTest
                        .Where(u => u.Equals(path))
                        .Any();

        Assert.IsTrue(result);
    }

    #endregion


    #region Valid me that Primitive type

    [Test]
    public static void ValidAPrimitiveType()
    {
        var myValue     = "change me to test";
        var valueType   = myValue.GetType();

        Assert.IsTrue(valueType.IsPrimitive);
    }

    #endregion
}