# to get the SHA256 checksum, replace the below command with the appropriate version:
# curl -Lf https://github.com/Azure/bicep/releases/download/v0.4.1/bicep-osx-x64 | shasum -a 256

class Bicep < Formula
  desc "Bicep: next generation template language for Azure Resource Manager (ARM)"
  homepage "https://github.com/Azure/bicep"
  version "0.4.1"
  url "https://github.com/Azure/bicep/releases/download/v0.4.1/bicep-osx-x64"
  sha256 "3ee9f7732601c2f6e5995e647ccb5044968c7f3f3ff130d931d1febf4c4acbc9"

  license "MIT"

  def install
    bin.install "bicep-osx-x64" => "bicep"
  end

  test do
  end
end
