using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dependator.Core.Tests
{
    [TestClass]
    public class ReferenceModelTests
    {
        [TestMethod]
        public void ReferenceModel_DefaultConstructor_InitializesProperties()
        {
            var model = new ReferenceModel();
            Assert.AreEqual(0, model.ReferenceFrom.Count());
            Assert.AreEqual(0, model.ReferenceTo.Count());
            Assert.AreEqual(0, model.References.Count());
        }
    }
}
