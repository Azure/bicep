# to get the SHA256 checksum, replace the below command with the appropriate version:
# curl -Lf https://github.com/Azure/bicep/releases/download/v0.4.451/bicep-osx-x64 | shasum -a 256

class Bicep < Formula
  desc "Bicep: next generation template language for Azure Resource Manager (ARM)"
  homepage "https://github.com/Azure/bicep"
  version "0.4.1008"
  url "https://github.com/Azure/bicep/releases/download/v0.4.1008/bicep-osx-x64"
  sha256 "c0872a6356787c29c2f95f67d5b9dfcfe07d95f8249f3fbd8bbee2a223e3024f"

  license "MIT"

  def install
    bin.install "bicep-osx-x64" => "bicep"
  end

  test do
  end
end
