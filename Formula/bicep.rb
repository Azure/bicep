# to get the SHA256 checksum, replace the below command with the appropriate version:
# curl -Lf https://github.com/Azure/bicep/releases/download/v0.4.451/bicep-osx-x64 | shasum -a 256

class Bicep < Formula
  desc "Bicep: next generation template language for Azure Resource Manager (ARM)"
  homepage "https://github.com/Azure/bicep"
  version "0.4.613"
  url "https://github.com/Azure/bicep/releases/download/v0.4.613/bicep-osx-x64"
  sha256 "829f7608c1a37dee402b8a075d780856477bf67ca4daac2120e8d96d4330cc9d"

  license "MIT"

  def install
    bin.install "bicep-osx-x64" => "bicep"
  end

  test do
  end
end
