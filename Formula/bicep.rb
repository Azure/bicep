# to get the SHA256 checksum, replace the below command with the appropriate version:
# curl -Lf https://github.com/Azure/bicep/releases/download/v0.4.412/bicep-osx-x64 | shasum -a 256

class Bicep < Formula
  desc "Bicep: next generation template language for Azure Resource Manager (ARM)"
  homepage "https://github.com/Azure/bicep"
  version "0.4.412"
  url "https://github.com/Azure/bicep/releases/download/v0.4.412/bicep-osx-x64"
  sha256 "7e31500ac6a1d23f6d085e19a01d263b38bfeb122f7843b16faa60ba4be943dd"

  license "MIT"

  def install
    bin.install "bicep-osx-x64" => "bicep"
  end

  test do
  end
end
