/*
 * Copyright 2017 Jan Tschada
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Statistics.Testing
{
    [TestClass]
    public class StringUtilsTesting
    {
        [TestMethod]
        public void TestSimilarity()
        {
            var text = "Licensed";
            var other = "License";
            var similarity = StringUtils.Similarity(text, other);
            Assert.IsTrue(similarity < 1.0f, "The two strings are not identical!");
            Assert.IsTrue(0.0f < similarity, "The two strings are similar!");
        }

        [TestMethod]
        public void TestSoundEx()
        {
            var text = StringUtils.SoundEx("Licensed", 4);
            var other = StringUtils.SoundEx("License", 4);
            var similarity = StringUtils.Similarity(text, other);
            Assert.IsTrue(similarity == 1.0f, "The two soundex codes are identical!");
        }
    }
}
