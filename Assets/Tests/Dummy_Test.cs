/**
 * @file Dummy_Test.cs
 * 
 * Unit tests for the Dummy class.
 */

using UnityEngine;
using System.Collections;
using SharpUnit;

public class Dummy_Test : TestCase 
{
    // Member values
    private Dummy m_dummy = null;   // Dummy instance for testing

    /**
     * Setup test resources, called before each test.
     */
    public override void SetUp()
    {
        m_dummy = new Dummy(); 
    }

    /**
     * Dispose of test resources, called after each test.
     */
    public override void TearDown()
    {
        m_dummy = null; 
    }

    /**
     * Sample test that passes.
     */
    [UnitTest]
    public void TestDummy_Pass()
    {
        Assert.NotNull(m_dummy);
    }

    /**
     * Sample test that fails.
     */
    //[UnitTest]
    //public void TestDummy_Fail()
    //{
       // Assert.Null(m_dummy);
    // }

    // @todo add more tests to test methods of the Dummy class...
	
}
