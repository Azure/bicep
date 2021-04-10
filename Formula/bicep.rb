# to get the SHA256 checksum, replace the below command with the appropriate version:
# curl -Lf https://github.com/Azure/bicep/releases/download/v0.3.255/bicep-osx-x64 | shasum -a 256

class Bicep < Formula
  desc "Bicep: next generation template language for Azure Resource Manager (ARM)"
  homepage "https://github.com/Azure/bicep"
  version "0.3.255"
  url "https://github.com/Azure/bicep/releases/download/v0.3.255/bicep-osx-x64"
  sha256 "2875f4c20c9fd49fa56d3c95dd610f64210e76ef1468be31035ec4895a044bd1"

  license "MIT"

  def install
    bin.install "bicep-osx-x64" => "bicep"
  end

  test do
  end
end
