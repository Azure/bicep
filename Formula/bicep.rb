# to get the SHA256 checksum, replace the below command with the appropriate version:
# curl -Lf https://github.com/Azure/bicep/releases/download/v0.3.1/bicep-osx-x64 | shasum -a 256

class Bicep < Formula
  desc "Bicep: next generation template language for Azure Resource Manager (ARM)"
  homepage "https://github.com/Azure/bicep"
  version "0.3.1"
  url "https://github.com/Azure/bicep/releases/download/v0.3.1/bicep-osx-x64"
  sha256 "89351c783d05ea00aad6ffedfce83ad9b5036313d76f4d255fc48fb030bcc9a7"

  license "MIT"

  def install
    bin.install "bicep-osx-x64" => "bicep"
  end

  test do
  end
end
